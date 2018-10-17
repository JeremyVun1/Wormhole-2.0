using System;
using System.Collections.Generic;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra
{
	public static class Util
	{
		//////////////////////////
		// Random number generation
		/////////////////////////
		//shorten the call for random number
		public static int Rand(int min, int max) {
			Random rng = new Random(Guid.NewGuid().GetHashCode());
			return rng.Next(min, max);
		}
		public static int Rand(int max) {
			Random rng = new Random(Guid.NewGuid().GetHashCode());
			return rng.Next(max);
		}
		public static int Rand() {
			Random rng = new Random(Guid.NewGuid().GetHashCode());
			return rng.Next();
		}

		//random return values
		public static float RandomInRange(MinMax<float> x) {
			float result = Rand((int)(x.Min * 1000), (int)(x.Max * 1000));
			result /= 1000f;
			return result;
		}

		public static Point2D RandomPointInRect(Rectangle rect) {
			int x = Rand((int)rect.X, (int)rect.Right);
			int y = Rand((int)rect.Y, (int)rect.Bottom);

			return SwinGame.PointAt(x, y);
		}

		public static Vector RandomUnitVector() {
			float x = (Rand(2000) - 1000);
			x /= 1000;
			float y = (Rand(2000) - 1000);
			y /= 1000;
			return SwinGame.VectorTo(x, y).UnitVector;
		}

		///////////////////////////////
		// Json deserialisation helpers
		//////////////////////////////
		public static JObject Deserialize(string filePath) {
			try {
				string buffer = File.ReadAllText(filePath);
				JObject obj = JsonConvert.DeserializeObject<JObject>(buffer);
				return obj;
			}
			catch (Exception e) {
				Console.WriteLine($"error deserializing from {filePath}");
				Console.WriteLine(e);
				return null;
			}
		}

		public static Color GetRGBColor(JToken color) {
			return SwinGame.RGBColor(color.Value<byte>("R"), color.Value<byte>("G"), color.Value<byte>("B"));
		}

		public static List<Color> LoadColors(JArray colorsObj) {
			List<Color> result = new List<Color>();

			foreach (JObject c in colorsObj) {
				result.Add(GetRGBColor(c));
			}
			return result;
		}

		public static Color DeserializeKeyedColor(JArray colorObj, string key) {
			foreach (JObject c in colorObj) {
				if (c.ContainsKey(key))
					return GetRGBColor(c.GetValue(key));
			}
			return Color.White;
		}
	}
}
