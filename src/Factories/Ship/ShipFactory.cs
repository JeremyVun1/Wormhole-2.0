using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Wormhole
{
	public class ShipFactory
	{
		public Dictionary<string, Shape> ShapeRegistry { get; private set; } //<ship Id, ship shape> for drawing ship objects to menu
		private Dictionary<string, string> fileRegistry; //<ship Id, filename>

		string[] fileList;
		private string resourceDir;

		public ShipFactory(string dir)
		{
			resourceDir = dir;
			fileList = Directory.GetFiles(resourceDir);
			ShapeRegistry = new Dictionary<string, Shape>();
			fileRegistry = new Dictionary<string, string>();
			RegisterShips();
		}

		//populate ship id and filepath for creation later
		public void RegisterShips()
		{
			BuildFileRegistry();
			BuildShapeRegistry();
		}

		private void BuildFileRegistry()
		{
			fileRegistry.Clear();
			
			foreach (string file in fileList)
			{
				JObject obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(file));
				fileRegistry.Add(obj.Value<string>("id"), file);
			}
		}

		private void BuildShapeRegistry()
		{
			ShapeRegistry.Clear();

			foreach (string file in fileList)
			{
				JObject obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(file));
				ShapeRegistry.Add(
					obj.Value<string>("id"),
					new Shape(obj.Value<JObject>("shape"), obj.Value<float>("scale"))
				);
			}
		}

		public IControllableShip CreatePlayerShip(string shipId)
		{
			string buffer = File.ReadAllText(fileRegistry[shipId]);
			dynamic obj = JsonConvert.DeserializeObject(buffer);

			return new PlayerShip(obj);
		}
	}
}
