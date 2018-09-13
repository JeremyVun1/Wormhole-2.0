using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class WHGameFactory
	{
		string dirPath;
		public WHGameFactory(string dir)
		{
			dirPath = dir;
		}

		//create new module
		public IGameModule Create(Player player, IShip ship, Level level)
		{
			IGameModule result = new WHGameModule(player, ship, level);

			return result;
		}
	}
}
