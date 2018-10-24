using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwinGameSDK;
using TaskForceUltra.src.GameModule;

namespace TaskForceUltra
{
	/// <summary>
	/// Level scene with background stars and a defined play area
	/// </summary>
	public class Level
	{
		public string Id { get; private set; }
		private List<EnvMod> EnvMods; //TODO implement
		private Background background;
		public Rectangle PlayArea { get; private set; }
		public bool Playable { get; private set; }
		public List<string> EntitiesToSpawn { get; private set; }

		public Level(string id, List<EnvMod> EnvMods, List<string> entToSpawn,
			bool playable, Rectangle playArea, Background bkgd)
		{
			Id = id;
			this.EnvMods = EnvMods;
			background = bkgd;
			PlayArea = playArea;
			EntitiesToSpawn = entToSpawn;
			Playable = playable;
		}

		public void Update() {
			background.Update();
		}

		public void Draw() {
			background.Draw();
		}
	}

	/// <summary>
	/// Level Factory
	/// </summary>
	public class LevelFactory
	{
		private string[] fileList;
		private string dirPath;
		public Dictionary<string,Level> levelList { get; private set; }

		public LevelFactory(string dirPath) {
			this.dirPath = dirPath;
			levelList = new Dictionary<string, Level>();
			RegisterLevels();
		}

		public void RegisterLevels() {
			levelList.Clear();

			fileList = Directory.GetFiles(dirPath);

			foreach (string file in fileList) {
				try {
					JObject obj = Util.Deserialize(file);
					if (obj == null)
						continue;

					string id = obj.Value<string>("id");
					bool playable = obj.Value<bool>("playable");
					string json = JsonConvert.SerializeObject(obj.GetValue("entities"));
					List<string> entitiesToSpawn = JsonConvert.DeserializeObject<List<string>>(json);

					Size2D<int> size = obj["size"].ToObject<Size2D<int>>();
					Rectangle playArea = SwinGame.CreateRectangle(SwinGame.PointAt(0, 0), size.W, size.H);

					List<EnvMod> EnvMods = obj["environMods"].ToObject<List<EnvMod>>(); //Most likely will not work

					JObject bkgdObj = obj.Value<JObject>("background");
					BackgroundFactory backgroundFac = new BackgroundFactory();
					Background bkgd = backgroundFac.Create(bkgdObj, playArea);

					levelList.Add(id, new Level(id, EnvMods, entitiesToSpawn, playable, playArea, bkgd));
				}
				catch(Exception e) {
					Console.WriteLine($"Cannot read level: {file}");
					Console.WriteLine(e);
				}
			}
		}

		//search methods on level list
		public bool LevelExists(string levelId) {
			if (levelList.ContainsKey(levelId))
					return true;
			return false;
		}

		public Level Fetch(string levelId) {
			return levelList[levelId];
		}
	}
}
