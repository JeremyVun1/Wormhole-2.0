using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class ShipList
	{
		public Dictionary<string, Ship> List { get; private set; } //<levelID><level obj>

		public ShipList()
		{
			List = new Dictionary<string, Ship>();
		}

		public void Add(Ship s)
		{
			List.Add(s.Id, s);
		}

		public Ship Fetch(string id)
		{
			return List[id];
		}

		public void Clear()
		{
			List.Clear();
		}
	}
}
