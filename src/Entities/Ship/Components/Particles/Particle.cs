using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using SwinGameSDK;

namespace Wormhole
{
	public class Particle : Component
	{
		private List<Color> colors;
		private float friction;
		private MinMax<float> velRange;
		private MinMax<float> lifetimeRange;
		private MinMax<float> turnRateRange;

		//public Emitter Emitter { get; set; }

		public override void Init(dynamic obj)
		{
			colors = new List<Color>();
			
			id = obj.id;
			foreach(var c in obj.colors)
			{
				colors.Add(SwinGame.RGBColor((byte)c.R, (byte)c.G, (byte)c.B));
			}
			Mass = 0;
			scale = obj.scale;
			shape = new Shape(obj.shape, scale);
			lifetimeRange = new MinMax<float>((float)obj.lifetimeRange.min, (float)obj.lifetimeRange.max);
			turnRateRange = new MinMax<float>((float)obj.turnRateRange.min, (float)obj.turnRateRange.max);
			velRange = new MinMax<float>((float)obj.velRange.min, (float)obj.velRange.max);
			friction = obj.friction;
		}
	}
}
