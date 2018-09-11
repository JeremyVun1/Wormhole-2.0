using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public class Background
	{
		private int StarCount { get; set; }
		private Color[] StarColor { get; set; }
		private Color BackgroundColor { get; set; }
		private Star[] StarArray { get; set; }

		public Background()
		{
			StarArray = new Star[StarCount];
		}

		public void Draw()
		{
			foreach(Star s in StarArray)
			{
				s.Draw();
			}
		}
	}
}
