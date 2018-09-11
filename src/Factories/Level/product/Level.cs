using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public class Level
	{
		public string id { get; set; }
		public Size2D<int> Size { get; set; }
		//public List<EnvMods> EnvironMods { get; set; }
		//public List<Mob> Entities { get; set; }
		public Background Background { get; set; }
		public int LevelNumber { get; set; }

		public Difficulty Difficulty { get; set; }

		public void Run()
		{
			Update();
			Draw();
		}

		public void Update()
		{
			//foreach(Entity e in Entities)
			//{
				//e.Update();
			//}
		}

		public void Draw()
		{
			Background.Draw();
			//foreach(Entity e in Entities)
			//{
				//e.Draw();
			//}
		}
	}
}
