using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwinGameSDK;

namespace TaskForceUltra
{
	public class Background
	{
		private List<Star> stars;
		private Color bkgdColor;

		public Background(List<Star> stars, Color bkgdColor)
		{
			this.stars = stars;
			this.bkgdColor = bkgdColor;
		}

		public void Update() {
			SwinGame.ClearScreen(bkgdColor);

			foreach(Star s in stars) {
				s.Update();
			}
		}

		public void Draw() {
			foreach(Star s in stars) {
				s.Draw();
			}
		}
	}

	/// <summary>
	/// Background Factory
	/// </summary>
	public class BackgroundFactory
	{
		public Background Create(JObject bkgdObj, Rectangle playArea) {
			//deserialise background properties
			//create stars
			int starCount = bkgdObj.Value<int>("starCount");
			JToken sizeObj = bkgdObj.GetValue("size");
			MinMax<int> sizeRange = new MinMax<int>(sizeObj.Value<int>("Min"), sizeObj.Value<int>("Max"));
			JToken dimObj = bkgdObj.GetValue("dimRate");
			MinMax<float> dimRange = new MinMax<float>(dimObj.Value<float>("Min"), dimObj.Value<float>("Max"));
			JToken flareObj = bkgdObj.GetValue("flareRate");
			MinMax<float> flareRange = new MinMax<float>(flareObj.Value<float>("Min"), flareObj.Value<float>("Max"));
			JArray starColorsObj = bkgdObj.Value<JArray>("colors");
			List<Color> starColors = Util.LoadColors(starColorsObj);

			StarFactory starFac = new StarFactory();
			List<Star> stars = starFac.CreateList(starCount, sizeRange, dimRange, flareRange, starColors, playArea);

			//create background
			JToken bkgdColorObj = bkgdObj.GetValue("backgroundColor");
			Color bkgdColor = Util.GetRGBColor(bkgdColorObj);

			return new Background(stars, bkgdColor);
		}
	}
}
