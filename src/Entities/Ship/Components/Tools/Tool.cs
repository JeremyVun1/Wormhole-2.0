using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public class Tool : Component
	{
		private List<Color> colors;
		private float cooldown;

		//public Emitter Emitter { get; set; }

		public override void Init(dynamic obj)
		{
			colors = new List<Color>();
			
			id = obj.id;
			foreach (var c in obj.colors)
			{
				colors.Add(SwinGame.RGBColor((byte)c.R, (byte)c.G, (byte)c.B));
			}
			cooldown = obj.cooldown;
			scale = obj.scale;
			shape = new Shape(obj.shape, scale);

			ChildComponents = new AmmoGroup(obj.Ammo);
		}
	}
}
