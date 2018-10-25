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
	/// <summary>
	/// Base class for ship components
	/// </summary>
	public abstract class Component : Mover
	{
		protected List<Component> childComponents;

		public override int Mass {
			get { return (base.Mass + childComponents.Mass()); }
		}
		public override List<LineSegment> DebrisLines { get { return null; } }

		public Component(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape, List<Color> colors,
			int health, Vector vel, Vector dir, BoundaryStrategy boundaryStrat, Team team, bool optimiseMe = false
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, boundaryStrat, team, optimiseMe)
		{
			childComponents = new List<Component>();
		}

		public override void Update() {
			base.Update();
			childComponents?.Update();
		}

		public override void Draw() {
			base.Draw();
			childComponents?.Draw();

			DebugDraw();
		}

		protected virtual void DebugDraw() {
			if (DebugMode.IsDebugging(Debugging.Component))
				Debug();
		}

		/// <summary>
		/// Ask component to move with specified velocity vector
		/// </summary>
		/// <param name="v">velocity vector</param>
		public void SetVel(Vector v) {
			Vel = v;
			childComponents?.SetVel(v);
		}

		/// <summary>
		/// Ask component to turn by specified radians
		/// </summary>
		/// <param name="theta"></param>
		public void Turn(double theta) {
			this.theta = theta;
			childComponents?.Turn(theta);
		}

		public override void Kill(Team killer) {
			base.Kill(Team.None);
		}
	}

	/// <summary>
	/// Component Factory
	/// </summary>
	public abstract class ComponentFactory
	{
		/// <summary>
		/// Create component from a filepath
		/// </summary>
		/// <param name="refObj">Json object containing the filepath</param>
		/// <param name="entHandler">entity handler</param>
		/// <param name="boundaryStrat">play area boundary behaviour</param>
		/// <param name="parentPos">position of object component is attached to</param>
		/// <param name="mod">modifier</param>
		/// <returns></returns>
		public virtual Component CreateFromReference(JObject refObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod = 1) {
			string path = SwinGame.AppPath() + refObj.Value<string>("path");

			//check that the path is valid
			if (!File.Exists(path)) {
				Console.WriteLine($"INVALID filepath: {path}");
				return null;
			}

			//load the full JObject from path
			refObj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
			return Create(refObj, path, entHandler, boundaryStrat, team, parentPos, mod);
		}

		public abstract Component Create(JObject compObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offsetPos, float mod = 1);

		public virtual List<Component> CreateList(JArray compObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod = 1) {
			List<Component> result = new List<Component>();

			foreach(JObject obj in compObj) {
				result.Add(CreateFromReference(obj, entHandler, boundaryStrat, team, parentPos));
			}

			return result;
		}
	}
}
