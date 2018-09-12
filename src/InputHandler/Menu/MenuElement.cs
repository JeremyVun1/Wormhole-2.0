using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public abstract class MenuElement : IMenuElement
	{
		protected Color hoverColor;
		protected Color fontColor;
		protected Color fillColor;
		protected Color borderColor;
		protected Rectangle boundingBox;

		protected Rectangle BuildBoundingBox(Point2D relPos, Size2D<float> relSize)
		{
			Point2D absPos = SwinGame.PointAt(relPos.X * SwinGame.ScreenWidth(), relPos.Y * SwinGame.ScreenHeight());
			Size2D<float> absSize = new Size2D<float>(relSize.W * SwinGame.ScreenWidth(), relSize.H * SwinGame.ScreenHeight());

			return SwinGame.CreateRectangle(absPos, absSize.W, absSize.H);
		}

		private Color getRGBColor(JToken color)
		{
			return SwinGame.RGBColor(color.Value<byte>("R"), color.Value<byte>("G"), color.Value<byte>("B"));
		}
		protected void LoadColors(JArray colorsObj)
		{
			foreach(JObject c in colorsObj)
			{
				if (c.ContainsKey("FontColor"))
				{
					fontColor = getRGBColor(c.GetValue("FontColor"));
					continue;
				}
				if (c.ContainsKey("FillColor"))
				{
					fillColor = getRGBColor(c.GetValue("FillColor"));
					continue;
				}
				if (c.ContainsKey("BorderColor"))
				{
					borderColor = getRGBColor(c.GetValue("BorderColor"));
					continue;
				}
				if (c.ContainsKey("HoverColor"))
				{
					hoverColor = getRGBColor(c.GetValue("HoverColor"));
					continue;
				}
			}
		}

		public abstract void Draw();

		public abstract void Update();
	}
}
