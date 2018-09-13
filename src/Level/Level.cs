using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class Level
	{
		[JsonProperty]
		public string Id { get; private set; }
		[JsonProperty]
		public bool Playable { get; private set; }
		[JsonProperty]
		private Size2D<int> Size { get; set; }
		[JsonProperty]
		private List<EnvMods> EnvironMods { get; set; }
		[JsonProperty]
		private int LevelNumber { get; set; }

		[JsonIgnore]
		private List<Mob> Entities { get; set; }
		[JsonIgnore]
		private Background Background { get; set; }		

		public Level(string json)
		{
			JsonConvert.PopulateObject(json, this);

			dynamic obj = JsonConvert.DeserializeObject(json);

			//build background
			Background = new Background(obj.Background, Size);

			//build entity list
			//TODO
			Entities = new List<Mob>();
		}

		public void Update()
		{
			Background.Update();

			foreach(Entity e in Entities)
			{
				e.Update();
			}
		}

		public void Draw()
		{
			Background.Draw();
			foreach(Entity e in Entities)
			{
				e.Draw();
			}
		}
	}
}
