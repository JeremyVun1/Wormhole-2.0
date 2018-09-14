using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	//TODO make this abstract and create weapon tool class
	public class Tool : Component
	{
		protected string ammoId;

		//TODO pass in ammoHandler reference so that we can spawn ammo and manage it
		public override void Init(JObject obj)
		{
			//cooldown handler
			cdHandler = new CooldownHandler(obj.Value<float>("cooldown"));

			//shape
			scale = obj.Value<float>("scale");
			shape = new Shape(obj.Value<JObject>("shape"), scale);

			//ammo
			ChildComponents = new AmmoGroup(obj.GetValue("ammo"));

			ammoId = ((AmmoGroup)ChildComponents).FetchAmmoId();

			base.Init(obj);
		}

		public override void Update()
		{
			cdHandler.Update();
		}

		public override void Draw(Point2D parentPos, Color parentColor)
		{
			base.Draw(parentPos, colors[0]);
		}
	}
}
