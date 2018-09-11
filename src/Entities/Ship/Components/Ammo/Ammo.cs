using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public class Ammo : Component
	{
		private float emissionRate;
		private int damage;
		private float lifetime;
		private float vel;
		private List<Color> colors;

		public override void Init(dynamic obj)
		{
			colors = new List<Color>();
			
			id = obj.id;
			damage = 1;
			Mass = obj.mass;
			scale = obj.scale;
			shape = new Shape(obj.shape, scale);
			lifetime = obj.lifetime;
			vel = obj.vel;
			foreach (var c in obj.colors)
			{
				colors.Add(SwinGame.RGBColor((byte)c.R, (byte)c.G, (byte)c.B));
			}
		}
	}
}
