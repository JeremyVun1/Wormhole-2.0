using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class TextBox : MenuElement
	{
		private string text;

		public TextBox(JToken txtJObj, JArray colors)
		{
			text = txtJObj.Value<string>("Text");
			LoadColors(colors);

			//build boundingBox with absolute values from relative position and size based on window width and height
			Point2D relPos = txtJObj["Pos"].ToObject<Point2D>();
			Size2D<float> relSize = txtJObj["Size"].ToObject<Size2D<float>>();
			boundingBox = BuildBoundingBox(relPos, relSize);
		}

		public override void Draw()
		{
			SwinGame.DrawText(text, fontColor, Color.Transparent, "MenuText", FontAlignment.AlignLeft, boundingBox);
		}

		public override void Update() { }
	}
}
