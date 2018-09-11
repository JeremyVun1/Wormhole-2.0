using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Wormhole
{
	public class LevelFactory
	{
		string[] fileList;
		private string levelDir;
		private LevelList levelList;

		public LevelFactory(string dir)
		{
			levelDir = dir;
			levelList = new LevelList();
		}

		public LevelList BuildLevelList()
		{
			//get list of all levels
			levelList.Clear();
			fileList = Directory.GetFiles(levelDir);
			string buffer;

			foreach (string file in fileList)
			{
				buffer = File.ReadAllText(file);
				levelList.Add(JsonConvert.DeserializeObject<Level>(buffer));
			}

			return levelList;
		}

		public Level Fetch(string LevelID)
		{
			return levelList.Fetch(LevelID);
		}
	}
}