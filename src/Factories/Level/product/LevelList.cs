using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class LevelList
	{
		public Dictionary<string, Level> List { get; private set; } //<levelID><level obj>

		public LevelList()
		{
			List = new Dictionary<string, Level>();
		}

		public void Add(Level l)
		{
			List.Add(l.id, l);
		}

		public Level Fetch(string id)
		{
			return List[id];
		}

		public void Clear()
		{
			List.Clear();
		}
	}
}
