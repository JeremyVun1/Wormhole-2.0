using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public class Star
	{
		private Color[] clr;
		private Rectangle shape;

		public Star()
		{
			shape = SwinGame.CreateRectangle(0, 0, 5, 5);
		}

		public void Draw()
		{
			SwinGame.FillRectangle(Blink(), shape);
		}

		private Color Blink()
		{
			return clr[SwinGame.Rnd(2)];
		}
	}
}
