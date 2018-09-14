using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class Background
	{
		private Color backgroundColor;
		private Star[] stars;
		public Size2D<int> playSize { get; private set; }

		public Background(JObject b, Size2D<int> s)
		{
			//get star colors
			List<Color> colors = new List<Color>();
			foreach (JObject c in b.GetValue("Colors"))
			{
				colors.Add(SwinGame.RGBColor(c.Value<byte>("R"), c.Value<byte>("G"), c.Value<byte>("B")));
			}

			//populate star objects
			stars = new Star[b.Value<int>("StarCount")];
			for (int i = 0; i < stars.Count(); ++i)
			{
				stars[i] = new Star(colors, b.GetValue("Size"), b.GetValue("DimRate"), b.GetValue("FlareRate"), s);
			}

			//Canvas color and size
			playSize = s;
			JToken bkgrdClr = b.GetValue("BackgroundColor");
			backgroundColor = SwinGame.RGBColor(bkgrdClr.Value<byte>("R"), bkgrdClr.Value<byte>("G"), bkgrdClr.Value<byte>("B"));
		}

		public void Update()
		{
			foreach(Star s in stars)
			{
				s.Update();
			}
		}

		public void Draw()
		{
			foreach(Star s in stars)
			{
				s.Draw();
			}
		}
	}
}
