using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TaskForceUltra.src.GameModule.AI;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Random Dummy Asteroid
	/// </summary>
	public class Asteroid : Mover, ICollides, IAIEntity
	{
		public int Damage { get; private set; }
		private AIStrategy aiStrat;
		public AIStrategy AIStrat { set { aiStrat = value; } }
		private float turnRate;
		public float MaxVel { get; private set; }
		private float thrustForce;
		private int mass;
		public override int Mass { get { return (base.Mass + mass); } }

		public Asteroid(string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int mass, int health, float thrustForce, Vector dir, float turnRate, BoundaryStrategy boundaryStrat,
			Team team, int dmg, bool optimiseMe = false)
		: base(id, filePath, refPos, offsetPos, shape, colors, health, SwinGame.VectorFromAngle(dir.Angle, thrustForce), dir, boundaryStrat, team, optimiseMe)
		{
			this.turnRate = turnRate;
			this.mass = mass;
			this.thrustForce = thrustForce;
			MaxVel = thrustForce;
			Vel = dir.Multiply(thrustForce);
			Damage = dmg;
		}

		public override void Update() {
			aiStrat.Update();
			base.Update();
		}

		public override void Draw() {
			base.Draw();

			DrawDebug();
		}

		private void DrawDebug() {
			if (DebugMode.IsDebugging(Debugging.Ship))
				Debug();
		}

		public bool TryReactToCollision(int dmg, Vector collidingVel, int collidingMass, Team collidingTeam, bool forceReaction = false) {
			health -= dmg;

			float velTransferMod = ((float)collidingMass / (float)Mass);
			Vel = Vel.AddVector(collidingVel.Multiply(velTransferMod));
			Vel = Vel.LimitToMagnitude(MaxVel);

			if (health <= 0)
				Kill(collidingTeam);

			return true;
		}

		public void Thrust(int thrustStrength) {
			Vector deltaV = Dir.Multiply(thrustForce / mass);
			deltaV = deltaV.Multiply(thrustStrength);
			Vel = (Vel.AddVector(deltaV)).LimitToMagnitude(MaxVel);
		}

		public void ForwardCommand() {
			Thrust(1);
		}

		public void TurnRightCommand() {
			theta += turnRate * Math.PI / 180;
		}

		public void TurnLeftCommand() {
			theta -= turnRate * Math.PI / 180;
		}

		public void Fire() { }

		public void BackwardCommand() {
			Thrust(-1);
		}

		public void StrafeLeftCommand() { }

		public void StrafeRightCommand() { }

		public void ShootCommand() { }

		public void ActivatePowerupCommand() { }
	}

	/// <summary>
	/// Asteroid Factory
	/// </summary>
	public class AsteroidFactory
	{
		private IHandlesEntities entHandler;

		public AsteroidFactory(IHandlesEntities entHandler) {
			this.entHandler = entHandler;
		}

		public Asteroid Create(string filePath, Rectangle playArea) {
			try {
				JObject obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath));
				string id = obj.Value<string>("id");
				List<Color> colors = Util.LoadColors(obj.Value<JArray>("colors"));
				int health = obj.Value<int>("baseHealth");
				int damage = obj.Value<int>("damage");
				int mass = obj.Value<int>("mass");
				string strategyName = obj.Value<string>("strategy");

				JToken sizeObj = obj.GetValue("sizeRange");
				JToken edgesObj = obj.GetValue("edgesRange");
				JToken turnRateRangeObj = obj.GetValue("turnRateRange");
				JToken velRangeObj = obj.GetValue("velRange");
				MinMax<float> sizeRange = new MinMax<float>(sizeObj.Value<float>("Min"), sizeObj.Value<float>("Max"));
				MinMax<float> edgesRange = new MinMax<float>(edgesObj.Value<float>("Min"), edgesObj.Value<float>("Max"));
				MinMax<float> turnRateRange = new MinMax<float>(turnRateRangeObj.Value<float>("Min"), turnRateRangeObj.Value<float>("Max"));
				MinMax<float> velRange = new MinMax<float>(velRangeObj.Value<float>("Min"), velRangeObj.Value<float>("Max"));

				Vector dir = Util.RandomUnitVector();
				float vel = Util.RandomInRange(velRange);
				float turnRate = Util.RandomInRange(turnRateRange);
				BoundaryStrategy boundaryStrat = new WrapBoundaryBehaviour(playArea);

				float size = Util.RandomInRange(sizeRange);
				mass *= (int)size / 10;
				Shape shape = new ShapeFactory().CreateCircleApprox(size, (int)Util.RandomInRange(edgesRange));
				Point2D spawnPos = Util.RandomPointInRect(playArea);

				Asteroid result = new Asteroid(id, filePath, SwinGame.PointAt(0, 0), SwinGame.PointAt(-size, size), shape, colors, mass, health, vel, dir, turnRate, boundaryStrat, Team.Computer, damage);
				result.TeleportTo(spawnPos);

				result.AIStrat = new AIStrategyFactory(0, 0).CreateByName(strategyName, result, entHandler);
				return result;
			}
			catch (Exception e) {
				Console.WriteLine(e);
				return null;
			}
		}
	}
}
