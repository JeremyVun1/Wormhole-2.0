using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class LevelList
	{
		public Dictionary<string, Level> levelList { get; private set; } //<levelID><level obj>

		public LevelList()
		{
			levelList = new Dictionary<string, Level>();
		}

		public void Add(Level l)
		{
			levelList.Add(l.Id, l);
		}

		public Level Fetch(string id)
		{
			return levelList[id];
		}

		public void Clear()
		{
			levelList.Clear();
		}
	}
}
