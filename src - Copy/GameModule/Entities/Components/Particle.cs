using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Particle object that doesn't really interact with anything else
	/// Impacts performance if too many of these things are on screen
	/// </summary>
	public class Particle : Component
	{
		private MinMax<float> lifetimeRange;
		private MinMax<float> velRange;
		private MinMax<float> turnRateRange;

		private float friction;
		private float thrustForce;
		private float turnRate;
		private float lifetime;

		private CooldownHandler cdHandler;

		public Particle(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, float friction, MinMax<float> lifetimeRange, MinMax<float> velRange,
			MinMax<float> turnRateRange, BoundaryStrategy boundaryStrat, Team team
		) : base(id, filePath, refPos, offsetPos, shape, colors, 1, SwinGame.VectorTo(0,0), SwinGame.VectorTo(0, -1), boundaryStrat, team, true)
		{
			this.friction = friction;
			this.lifetimeRange = lifetimeRange;
			this.velRange = velRange;
			this.turnRateRange = turnRateRange;

			thrustForce = 0;
			turnRate = 0;
			lifetime = 0;
		}

		public override void Update() {
			if (cdHandler != null) {
				if (cdHandler.IsOnCooldown()) {
					Vel = Vel.Multiply(friction);
					turnRate *= friction;
					theta = turnRate;

					colorIndex = SwinGame.Rnd(colors.Count);
				}
				else Kill(Team.None);
			}

			base.Update();
		}

		public override void Draw() {
			if (cdHandler != null)
				base.Draw();

			DebugDraw();
		}

		/// <summary>
		/// Initialise the particle
		/// </summary>
		/// <param name="pos">spawning position</param>
		/// <param name="dir">spawning direction</param>
		public void Init(Point2D pos, Vector dir) {
			thrustForce = Util.RandomInRange(velRange);
			turnRate = Util.RandomInRange(turnRateRange);
			lifetime = Util.RandomInRange(lifetimeRange);
			cdHandler = new CooldownHandler(lifetime * 1000);
			cdHandler.StartCooldown();

			TeleportTo(pos);
			double theta = Dir.AngleTo(dir) * Math.PI / 180;
			this.theta = theta;

			Vector deltaV = dir.Multiply(-thrustForce);
			Vel = (Vel.AddVector(deltaV)).LimitToMagnitude(thrustForce);
		}

		protected override void DebugDraw() {
			if (DebugMode.IsDebugging(Debugging.Particle))
				Debug();
		}
	}


	/// <summary>
	/// Particle Factory
	/// </summary>
	public class ParticleFactory : ComponentFactory
	{
		public override Component CreateFromReference(JObject particleObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod =1) {
			string path = SwinGame.AppPath() + particleObj.Value<string>("path");

			return base.CreateFromReference(particleObj, entHandler, boundaryStrat, team, parentPos);
		}

		public override Component Create(JObject particleObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offsetPos, float mod =1)
		{
			string id = particleObj.Value<string>("id");
			List<Color> colors = Util.LoadColors(particleObj.Value<JArray>("colors"));
			float scale = particleObj.Value<float>("scale");
			JObject shapeObj = particleObj.Value<JObject>("shape");
			Shape shape = new ShapeFactory().Create(shapeObj, scale, offsetPos);
			float friction = particleObj.Value<float>("friction");
			MinMax<float> lifetimeRange = particleObj["lifetimeRange"].ToObject<MinMax<float>>();
			MinMax<float> velRange = particleObj["velRange"].ToObject<MinMax<float>>();
			MinMax<float> turnRateRange = particleObj["turnRateRange"].ToObject<MinMax<float>>();

			return new Particle(id, path, SwinGame.PointAt(0, 0), offsetPos, shape, colors,
				friction, lifetimeRange, velRange, turnRateRange, boundaryStrat, team);
		}
	}
}
