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
	/// Ship tool that fires ammo
	/// </summary>
	public class Tool : Component
	{
		private float cooldown;
		public Ammo Ammo { get { return childComponents.OfType<Ammo>().First(); } }
		private IHandlesEntities entHandler;

		private int mass;
		public override int Mass { get { return base.Mass + mass; } }

		private CooldownHandler cdHandler;

		public Tool(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int health, Vector vel, Vector dir, float cooldown, BoundaryStrategy boundaryStrat,
			Team team, List<Component> children, int mass, IHandlesEntities entHandler
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, boundaryStrat, team)
		{
			this.cooldown = cooldown;
			this.mass = mass <= 0 ? 1 : mass;
			childComponents = children;
			this.entHandler = entHandler;
			cdHandler = new CooldownHandler(cooldown*1000);
			cdHandler.StartCooldown();
		}

		/// <summary>
		/// activate the tool and fire it's ammo
		/// </summary>
		public void Activate() {
			if (!cdHandler.IsOnCooldown()) {
				JObject ammoObj = Util.Deserialize(Ammo.FilePath);

				Ammo newAmmo = new AmmoFactory().Create(ammoObj, FilePath, entHandler, boundaryStrat, Team, SwinGame.PointAt(0,0)) as Ammo;
				newAmmo.TeleportTo(RealPos);

				newAmmo.Init(RealPos, Dir, Vel);
				entHandler.Track(newAmmo);
				cdHandler.StartCooldown();
			}
		}

		public override void Update() {
			base.Update();
			Ammo.Sleep();
		}

		public override void Draw() {
			base.Draw();
			DrawCooldown();
		}

		private void DrawCooldown() {
			Point2D offset1 = SwinGame.PointAt(-6, 5);
			Point2D offset2 = SwinGame.PointAt(6, 10);
			Rectangle cdBar = SwinGame.CreateRectangle(RealPos.Add(offset1), RealPos.Add(offset2));
			Rectangle cdProg = SwinGame.CreateRectangle(cdBar.TopLeft, cdBar.Width * cdHandler.cdPercentage, cdBar.Height);

			SwinGame.FillRectangle(SwinGame.RGBAColor(0,255,0,140), cdProg);
			SwinGame.DrawRectangle(SwinGame.RGBAColor(0,255,0,140), cdBar);
		}

		public override void TeleportTo(Point2D target) {
			base.TeleportTo(target);
			childComponents?.TeleportTo(target);
		}
	}

	/// <summary>
	/// Tool factory
	/// </summary>
	public class ToolFactory : ComponentFactory
	{
		public override Component CreateFromReference(JObject toolObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod =1) {
			string path = SwinGame.AppPath() + toolObj.Value<string>("path");
			Point2D offsetPos = toolObj["pos"].ToObject<Point2D>().Multiply(10);
			offsetPos = offsetPos.Add(parentPos);

			return base.CreateFromReference(toolObj, entHandler, boundaryStrat, team, offsetPos);
		}

		public override Component Create(JObject toolObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offsetPos, float mod =1) 
		{
			string id = toolObj.Value<string>("id");
			int mass = toolObj.Value<int>("mass");
			float scale = toolObj.Value<float>("scale");
			float cooldown = toolObj.Value<float>("cooldown") / mod;
			JObject shapeObj = toolObj.Value<JObject>("shape");
			List<Color> colors = Util.LoadColors(toolObj.Value<JArray>("colors"));
			Shape shape = new ShapeFactory().Create(shapeObj, scale, offsetPos);

			JObject ammoObj = toolObj.Value<JObject>("ammo");
			Component ammo = new AmmoFactory().CreateFromReference(ammoObj, entHandler, boundaryStrat, team, offsetPos, mod);

			return new Tool(id, path, SwinGame.PointAt(0, 0), offsetPos, shape,
				colors, 1, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1), cooldown,
				boundaryStrat, team, new List<Component>() { ammo }, mass, entHandler);
		}
	}
}
