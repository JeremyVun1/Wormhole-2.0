using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra.src.GameModule.Entities
{
	public class Engine : Component
	{
		public float Thrust { get; private set; }
		public float MaxVel { get; private set; }
		private float turnRate;

		private int mass;
		public override int Mass { get { return base.Mass + mass; } }
		private bool thrusting;

		public Engine(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int health, Vector vel, Vector dir, BoundaryStrategy boundaryStrat,
			Team team, List<Component> components, float thrust, float maxVel, float turnRate, int mass
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, boundaryStrat, team)
		{
			this.mass = mass <= 0 ? 1 : mass;
			Thrust = thrust;
			MaxVel = maxVel;
			this.turnRate = turnRate;
			childComponents = components;
		}

		//return clamped vector modified by vDir
		public Vector ApplyForce(Vector vDir, Vector vel, int Mass) {
			//apply force to the vector
			Vector modified = vel;
			Vector force = new Vector {
				X = vDir.X * (Thrust / Mass),
				Y = vDir.Y * (Thrust / Mass)
			};
			modified += force;

			if (modified.Magnitude > MaxVel)
				return vel;
			else {
				thrusting = true;
				return modified;
			}
		}

		public double Turn(float turnStrength) {
			return (turnRate * turnStrength);
		}

		public override void Update() {
			if (thrusting) {
				foreach(Emitter e in childComponents.OfType<Emitter>()) {
					e.Activate();
				}
				thrusting = false;
			}

			base.Update();
		}

		public override void TeleportTo(Point2D target) {
			base.TeleportTo(target);
			childComponents?.TeleportTo(target);
		}
	}

	/// <summary>
	/// Engine Factory
	/// </summary>
	public class EngineFactory : ComponentFactory
	{
		public override Component CreateFromReference(JObject engineObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod =1) {
			string path = SwinGame.AppPath() + engineObj.Value<string>("path");
			Point2D offsetPos = engineObj["pos"].ToObject<Point2D>().Multiply(10);

			return base.CreateFromReference(engineObj, entHandler, boundaryStrat, team, offsetPos);
		}

		public override Component Create(JObject engineObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offsetPos, float mod = 1)
		{
			string id = engineObj.Value<string>("id");
			float thrust = engineObj.Value<float>("thrust") * mod;
			float maxVel = engineObj.Value<float>("maxVel") * mod;
			float turnRate = engineObj.Value<float>("turnRate") * mod;
			int mass = engineObj.Value<int>("mass");
			float scale = engineObj.Value<float>("scale");
			JObject shapeObj = engineObj.Value<JObject>("shape");
			Shape shape = new ShapeFactory().Create(shapeObj, scale, offsetPos);

			JArray emitterObj = engineObj.Value<JArray>("emitters");
			List<Component> emitters = new EmitterFactory().CreateList(emitterObj, entHandler, boundaryStrat, team, offsetPos);

			return new Engine(id, path, SwinGame.PointAt(0, 0), offsetPos, shape,
				new List<Color> { Color.White }, 1, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1),
				boundaryStrat, team, emitters, thrust, maxVel, turnRate, mass);
		}
	}
}
