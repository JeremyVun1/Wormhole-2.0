using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaskForceUltra.src.GameModule.Entities;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.AI;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Spaceship object that has engines, tools, emitters
	/// </summary>
	public abstract class Ship : Mover, ICollides
	{
		private List<Component> componentList;

		protected int hurtThreshhold;
		protected Timer hurtTimer;
		protected bool isHurting;

		public float Accel {
			get {
				float result = 0;
				foreach(Engine e in componentList?.OfType<Engine>()) {
					result += e.Thrust;
				}
				return result;
			}
		}

		public float MaxVel {
			get {
				float result = 0;
				foreach (Engine e in componentList?.OfType<Engine>()) {
					result = e.MaxVel > result ? e.MaxVel : result;
				}
				return result;
			}
		}

		public int Damage { get; private set; }

		public override int Mass {
			get { return (base.Mass + componentList.Mass()); }
		}

		public Ship(
			string id, string filePath, Point2D refPos, Point2D offsetPos,
			Shape shape, List<Color> colors, int health, Vector vel, Vector dir, int threshhold,
			BoundaryStrategy boundaryStrat, Team team, List<Component> components
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, boundaryStrat, team)
		{
			componentList = components;
			Damage = 1;
			hurtTimer = SwinGame.CreateTimer();
			isHurting = false;
			hurtThreshhold = threshhold;
		}

		public override void Update() {
			base.Update();
			componentList?.Update();

			HandleHurtingState();

			TeleportTo(RealPos);
		}

		/// <summary>
		/// Handles damage timeouts to prevent things from losing all their health really quickly
		/// </summary>
		private void HandleHurtingState() {
			if (isHurting && hurtTimer.Ticks > hurtThreshhold) {
				hurtTimer.Stop();
				isHurting = false;
				colorIndex = 0;
			}
			else if (isHurting) {
				colorIndex = Util.Rand(colors.Count);
			}
		}

		public override void Draw() {
			base.Draw();
			componentList?.Draw();
		}

		/// <summary>
		/// Object's reaction to being collided with
		/// </summary>
		/// <param name="dmg">incoming damage</param>
		/// <param name="collidingVel">velocity of other object</param>
		/// <param name="collidingMass">mass of other object</param>
		/// <param name="collider">team of other object</param>
		/// <param name="forceReaction">opt to bypass hurting timeout</param>
		public virtual void ReactToCollision(int dmg, Vector collidingVel, int collidingMass, Team collidingTeam, bool forceReaction = false) {
			if (!isHurting || forceReaction) {
				isHurting = true;
				hurtTimer.Start();

				health -= dmg;
				float velTransferMod = ((float)collidingMass / (float)Mass);
				Vel = Vel.AddVector(collidingVel.Multiply(velTransferMod));
				Vel = Vel.LimitToMagnitude(MaxVel);
			}

			if (health <= 0)
				Kill(collidingTeam);
		}


		//////////////////////
		// MOVEMENT API
		//////////////////////
		/// <summary>
		/// Ask ship engines to thrust ship along a passed in vector direction
		/// magnitude of vector direction determines thrust scaling
		/// </summary>
		public void Thrust(Vector vDir) {
			vDir = vDir.Rotate(Dir.Angle * (Math.PI / 180));

			foreach (Engine e in componentList?.OfType<Engine>()) {
				Vel = e.ApplyForce(vDir, Vel, Mass);
				componentList.SetVel(Vel);
			}
		}

		/// <summary>
		/// Ask engines to turn the ship based on a positive or negative value
		/// </summary>
		protected void Turn(float turnStrength) {
			foreach (Engine e in componentList?.OfType<Engine>()) {
				theta = e.Turn(turnStrength);
				componentList.Turn(theta);
			}
		}

		/// <summary>
		/// Turn the ship to a specified vector
		/// </summary>
		/// <param name="targetDir">vector to turn to</param>
		/// <param name="turnStrength">opt to modify turn strength</param>
		public void TurnTo(Vector targetDir, float turnStrength = 1) {
			//turn towards the target direction as much as our engines will allow us
			double desiredTheta = Dir.AngleTo(targetDir) * Math.PI/180;

			foreach (Engine e in componentList?.OfType<Engine>()) {
				double engineTheta = e.Turn(turnStrength);
				theta += engineTheta;
			}

			theta *= desiredTheta.GetSign();
			if (theta <= 0) {
				theta = theta.Clamp(desiredTheta, 0);
			} else {
				theta = theta.Clamp(0, desiredTheta);
			}

			componentList.Turn(theta);
		}

		/// <summary>
		/// Activate a specific tool
		/// </summary>
		/// <param name="toolId">tool id to activate</param>
		public void Fire(string toolId) {
			foreach (Tool t in componentList?.OfType<Tool>()) {
				if (t.Id == toolId)
					t.Activate();
			}
		}

		/// <summary>
		/// Activate all tools
		/// </summary>
		public void Fire() {
			foreach (Tool t in componentList?.OfType<Tool>()) {
				t.Activate();
			}
		}

		/// <summary>
		/// Teleport the ship to the target position
		/// </summary>
		public override void TeleportTo(Point2D target) {
			base.TeleportTo(target);
			componentList?.TeleportTo(target);
		}

		public void SetTeam(Team team) {
			Team = team;
		}
	}


	/// <summary>
	/// Ship Factory
	/// </summary>
	public class ShipFactory
	{
		public Dictionary<string, Shape> ShapeRegistry { get; private set; }
		public Dictionary<string, string> FileRegistry { get; private set; }
		private string[] fileList;

		private string dirPath;

		public ShipFactory(string dirPath) {
			this.dirPath = dirPath;
			fileList = Directory.GetFiles(dirPath);
			ShapeRegistry = new Dictionary<string, Shape>();
			FileRegistry = new Dictionary<string, string>();
			RegisterShips();
		}

		/// <summary>
		/// Register ships from file
		/// </summary>
		public void RegisterShips() {
			BuildFileRegistry();
			BuildShapeRegistry();
		}

		/// <summary>
		/// Ship id, filename pairs
		/// </summary>
		private void BuildFileRegistry() {
			try {
				FileRegistry.Clear();

				foreach (string file in fileList) {
					JObject obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(file));
					string id = obj?.Value<string>("id");
					if (id != null && !FileRegistry.ContainsKey(id))
						FileRegistry.Add(id, file);
				}
			} catch (Exception e) {
				Console.WriteLine($"invalid file {e}");
			}
		}

		/// <summary>
		/// ship id, ship shape pairs
		/// useful if we only want to draw the shapes
		/// </summary>
		private void BuildShapeRegistry() {
			try {
				ShapeRegistry.Clear();

				foreach (string file in fileList) {
					JObject obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(file));
					string id = obj?.Value<string>("id");

					//build the ship's shape
					ShapeFactory shapeFac = new ShapeFactory();
					float scale = obj.Value<float>("scale");
					Shape shape = shapeFac.Create(obj.Value<JObject>("shape"), scale, SwinGame.PointAt(0, 0)); //done

					if (id != null && shape != null && !ShapeRegistry.ContainsKey(id))
						ShapeRegistry.Add(id, shape);
				}
			} catch (Exception e) {
				Console.WriteLine($"invalid file {e}");
			}
		}

		/// <summary>
		/// Return the file path of the specified ship id
		/// </summary>
		/// <param name="id">ship id</param>
		/// <returns>ship file path</returns>
		public string FetchShipPath(string id) {
			return FileRegistry?[id];
		}

		private List<Component> BuildComponents(JArray enginesObj, JArray toolsObj, JArray emittersObj,
			IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offset, float mod = 1) {
			List<Component> result = new List<Component>();

			result.AddRange(new EngineFactory().CreateList(enginesObj, entHandler, boundaryStrat, team, offset, mod));
			result.AddRange(new ToolFactory().CreateList(toolsObj, entHandler, boundaryStrat, team, offset, mod));
			result.AddRange(new EmitterFactory().CreateList(emittersObj, entHandler, boundaryStrat, team, offset, mod=1));

			return result;
		}

		private PlayerShip CreatePlayerShip(string shipId, Point2D pos, BoundaryStrategy boundaryStrat, ControllerType controller, IHandlesEntities entHandler) {
			JObject obj = Util.Deserialize(FileRegistry[shipId]);

			int health = obj.Value<int>("health");
			List<Color> shipColors = new List<Color> { Util.GetRGBColor(obj.GetValue("color")), Color.Yellow, Color.White, Color.Red };
			float scale = obj.Value<float>("scale");
			JArray enginesObj = obj.Value<JArray>("engines");
			JArray toolsObj = obj.Value<JArray>("tools");
			JArray emittersObj = obj.Value<JArray>("emitters");
			JObject shapeObj = obj.Value<JObject>("shape");

			Team team = (Team)(int)controller;
			Point2D offset = SwinGame.PointAt(0, 0);

			//shape
			Shape shape = new ShapeFactory().Create(shapeObj, scale, SwinGame.PointAt(0, 0));
			//shape.TeleportTo(pos);

			//component
			List<Component> components = BuildComponents(enginesObj, toolsObj, emittersObj, entHandler, boundaryStrat, team, offset);

			PlayerShip result = new PlayerShip(shipId, FileRegistry[shipId], pos, SwinGame.PointAt(0, 0), shape, shipColors,
				health, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1), boundaryStrat, team, components);

			result.TeleportTo(pos);
			return result;
		}

		private AIShip CreateAIShip(string shipId, Point2D pos,BoundaryStrategy boundaryStrat, Difficulty diff, IHandlesEntities entHandler) {
			AIStrategyFactory strategyFac = new AIStrategyFactory(diff.DifficultyLevel, diff.ShootCooldown);

			JObject obj = Util.Deserialize(FileRegistry[shipId]);

			int health = obj.Value<int>("health");
			List<Color> shipColors = new List<Color> { Color.Crimson, Color.Yellow, Color.White, Color.Red };
			float scale = obj.Value<float>("scale");
			JArray enginesObj = obj.Value<JArray>("engines");
			JArray toolsObj = obj.Value<JArray>("tools");
			JArray emittersObj = obj.Value<JArray>("emitters");
			JObject shapeObj = obj.Value<JObject>("shape");

			Team team = Team.Computer;
			Point2D offset = SwinGame.PointAt(0, 0);

			//shape
			Shape shape = new ShapeFactory().Create(shapeObj, scale, SwinGame.PointAt(0, 0));
			shape.TeleportTo(pos);

			//components
			List<Component> components = BuildComponents(enginesObj, toolsObj, emittersObj, entHandler, boundaryStrat, team, offset, diff.AIMod);

			//build and return ship
			AIShip result = new AIShip(shipId, FileRegistry[shipId], pos, SwinGame.PointAt(0, 0), shape, shipColors,
				health, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1), boundaryStrat, team, components);

			//create strategy
			AIStrategy aiStrat = strategyFac.Create((IAIEntity)result, entHandler);
			result.AIStrategy = aiStrat;

			result.TeleportTo(pos);
			return result;
		}

		/// <summary>
		/// Create specified ship
		/// </summary>
		/// <param name="shipId">ship id</param>
		/// <param name="pos">spawn position</param>
		/// <param name="boundaryStrat">behaviour at the play area boundary</param>
		/// <param name="controller">computer/players</param>
		/// <param name="diff">difficulty setting</param>
		/// <param name="entHandler">entity handler</param>
		/// <returns>player ship or ai ship depending on controller</returns>
		public Ship Create(string shipId, Point2D pos, BoundaryStrategy boundaryStrat, ControllerType controller, Difficulty diff, IHandlesEntities entHandler) {
			if (!FileRegistry.ContainsKey(shipId))
				return null;

			switch(controller) {
				case ControllerType.Computer:
					return CreateAIShip(shipId, pos, boundaryStrat, diff, entHandler);
				case ControllerType.Player1:
				case ControllerType.Player2:
				case ControllerType.Player3:
				case ControllerType.Player4:
					return CreatePlayerShip(shipId, pos, boundaryStrat, controller, entHandler);
				default: return null;
			}
		}

		/// <summary>
		/// Create a random ship!
		/// </summary>
		/// <returns>a ship</returns>
		public Ship CreateRandomShip(Point2D pos, BoundaryStrategy boundaryStrat, ControllerType controller, Difficulty diff, IHandlesEntities entHandler) {
			int i = Util.Rand(FileRegistry.Count);
			string randomShipId = FileRegistry.ElementAt(i).Key;
			return Create(randomShipId, pos, boundaryStrat, controller, diff, entHandler);
		}
	}
}
