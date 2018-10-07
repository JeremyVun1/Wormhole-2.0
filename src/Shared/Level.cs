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
	public class Level
	{
		public string Id { get; private set; }
		private List<EnvMod> EnvMods; //TODO implement
		private Background background;
		public Rectangle PlayArea { get; private set; }
		public int LevelNumber { get; private set; } //TODO implement

		public Level(string id, List<EnvMod> EnvMods, int num,
			Rectangle playArea, Background bkgd)
		{
			Id = id;
			this.EnvMods = EnvMods;
			background = bkgd;
			PlayArea = playArea;
			LevelNumber = num;
		}

		public void Update() {
			background.Update();
		}

		public void Draw() {
			background.Draw();
			//SwinGame.FillRectangle(SwinGame.RGBAColor(150, 150, 150, 100), PlayArea);
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
				JObject obj = Util.Deserialize(file);
				if (obj == null)
					continue;

				string id = obj.Value<string>("id");
				int num = obj.Value<int>("levelNumber");

				Size2D<int> size = obj["size"].ToObject<Size2D<int>>();
				Rectangle playArea = SwinGame.CreateRectangle(SwinGame.PointAt(0, 0), size.W, size.H);

				List<EnvMod> EnvMods = obj["environMods"].ToObject<List<EnvMod>>(); //Most likely will not work

				JObject bkgdObj = obj.Value<JObject>("background");
				BackgroundFactory backgroundFac = new BackgroundFactory();
				Background bkgd = backgroundFac.Create(bkgdObj, playArea);

				levelList.Add(id, new Level(id, EnvMods, num, playArea, bkgd));
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
