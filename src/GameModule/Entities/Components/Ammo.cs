using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TaskForceUltra.src.GameModule.AI;
using TaskForceUltra.src.GameModule.AI.strategies;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Ammo object
	/// </summary>
	public class Ammo : Component, ICollides, IAIEntity
	{
		private float lifetime;
		private int mass;
		private float thrustForce;
		public float MaxVel { get; private set; }
		protected float turnRate;
		protected bool thrusting;

		private AIStrategy aiStrat;
		public AIStrategy AIStrat { set { aiStrat = value; } }

		public new int Damage { get; private set; }
		public override int Mass { get { return base.Mass + mass; } }
		public override List<LineSegment> DebrisLines { get { return Shape.GetLines(2); } }

		protected bool sleep;
		private CooldownHandler cdHandler;

		public Ammo(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int mass, int damage, float lifetime, float vel, float maxVel,
			float turnRate, BoundaryStrategy boundaryStrat, Team team
		) : base(id, filePath, refPos, offsetPos, shape, colors, 1, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1), boundaryStrat, team)
		{
			Damage = damage;
			this.lifetime = lifetime < 0 ? 0 : lifetime;
			this.mass = mass <= 0 ? 1 : mass;

			MaxVel = maxVel;
			thrustForce = vel;
			this.turnRate = turnRate;
		}

		/// <summary>
		/// initialise an ammo object
		/// </summary>
		/// <param name="pos">spawning position</param>
		/// <param name="dir">spawning direction</param>
		/// <param name="vel">spawning velocity</param>
		public virtual void Init(Point2D pos, Vector dir, Vector vel) {
			//set position and direction of ammo to that of the passed in parent entity
			TeleportTo(pos);
			theta = Dir.AngleTo(dir) * Math.PI / 180;

			MaxVel += vel.Magnitude;
			cdHandler = new CooldownHandler(lifetime * 1000);
			cdHandler.StartCooldown();
		}

		public override void Update() {
			if (sleep)
				return;

			aiStrat.Update();
			base.Update();

			//kill ammo if expired
			if (cdHandler != null) {
				if (!cdHandler.IsOnCooldown())
					Kill(Team.None);
			}
		}

		public override void Draw() {
			if (sleep)
				return;

			if (cdHandler != null)
				base.Draw();

			DebugDraw();
		}

		protected override void DebugDraw() {
			if (DebugMode.IsDebugging(Debugging.Ammo))
				Debug();
		}

		public virtual void ForwardCommand() {
			Thrust(1);
		}

		public void BackwardCommand() {
			Thrust(-1);
		}

		/// <summary>
		/// Accelerate the Ammo object
		/// </summary>
		/// <param name="thrustStrength">negative is backwards, positive is forward</param>
		private void Thrust(int thrustStrength) {
			thrusting = true;
			Vector deltaV = Dir.Multiply(thrustForce / mass);
			deltaV = deltaV.Multiply(thrustStrength);
			Vel = (Vel.AddVector(deltaV)).LimitToMagnitude(MaxVel);
		}

		/// <summary>
		/// Turn Right
		/// </summary>
		public void TurnRightCommand() {
			theta += turnRate * Math.PI / 180;
		}

		/// <summary>
		/// Turn Left
		/// </summary>
		public void TurnLeftCommand() {
			theta -= turnRate * Math.PI / 180;
		}

		public void Fire() { }

		public void StrafeLeftCommand() { }

		public void StrafeRightCommand() { }

		public void ShootCommand() { }

		public void ActivatePowerupCommand() { }

		public override bool TryReactToCollision(int dmg, Vector collidingVel, int collidingMass, Team collider, bool forceReaction = false) {
			if (!sleep)
				Kill(Team.None);
			return false;
		}

		/// <summary>
		/// Deactivate ammo
		/// </summary>
		public void Sleep() {
			sleep = true;
		}
	}

	/// <summary>
	/// Ammo Factory
	/// </summary>
	public class AmmoFactory : ComponentFactory
	{
		public override Component Create(JObject ammoObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod = 1) {
			try {
				string id = ammoObj.Value<string>("id");
				List<Color> colors = Util.LoadColors(ammoObj.Value<JArray>("colors"));
				int mass = ammoObj.Value<int>("mass");
				int damage = (int)(ammoObj.Value<int>("damage") * mod);
				float lifetime = ammoObj.Value<float>("lifetime") * mod;
				float vel = ammoObj.Value<float>("vel") * mod;
				float maxVel = ammoObj.Value<float>("maxVel") * mod;
				float turnRate = ammoObj.Value<float>("turnRate") * mod;
				float scale = ammoObj.Value<float>("scale");
				JObject shapeObj = ammoObj.Value<JObject>("shape");
				Shape shape = new ShapeFactory().Create(shapeObj, scale, parentPos);
				string strategy = ammoObj.Value<string>("strategy");

				float primingDelay = 0;
				try { primingDelay = ammoObj.Value<float>("primingDelay"); }
				catch { }

				if (team == Team.Computer)
					colors = new List<Color> { Color.Yellow };

				JArray emitterObj = null;
				try { emitterObj = ammoObj.Value<JArray>("emitters"); } catch { }

				Ammo result;

				if (emitterObj != null) {
					List<Component> emitters = new EmitterFactory().CreateList(emitterObj, entHandler, boundaryStrat, team, parentPos, mod);
					result = new EmittingAmmo(id, path, SwinGame.PointAt(0, 0), parentPos, shape, colors, mass, damage, lifetime, vel, maxVel, primingDelay, turnRate, emitters, boundaryStrat, entHandler, team);
				} else {
					result = new Ammo(id, path, SwinGame.PointAt(0, 0), parentPos, shape, colors, mass, damage, lifetime, vel, maxVel, turnRate, boundaryStrat, team);
				}

				AIStrategyFactory aiStratFac = new AIStrategyFactory(0, 0);
				result.AIStrat = aiStratFac.CreateByName(strategy, result, entHandler);
				return result;
			}
			catch (Exception e) {
				Console.WriteLine(e);
				return null;
			}
		}
	}
}
