using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class SelectShipCommand : ICommand
	{
		MenuModule menuModule;
		string shipId;

		public SelectShipCommand(MenuModule m, string id)
		{
			menuModule = m;
			shipId = id;
		}

		public void Execute()
		{
			menuModule.SelectShip(shipId);
		}
	}
}
