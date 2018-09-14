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

		private List<Entity> entities;
		private Background background;
		private DifficultyType difficulty;
		public bool Ended { get; private set; }
		public Size2D<int> PlaySize
		{
			get { return background.playSize; }
		}

		public Level(string json)
		{
			JsonConvert.PopulateObject(json, this);

			dynamic obj = JsonConvert.DeserializeObject(json);

			//build background
			background = new Background(obj.Background, Size);

			//build entity list
			//TODO
			entities = new List<Entity>();
		}

		public void AddEntity(Entity e)
		{
			entities.Add(e);
		}

		public void SetDifficulty(DifficultyType d)
		{
			difficulty = d;	
		}

		public void Update()
		{
			background.Update();

			foreach(Entity e in entities)
			{
				e.Update();
			}
		}

		public void Draw()
		{
			background.Draw();
			foreach(Entity e in entities)
			{
				e.Draw();
			}
		}
	}
}
