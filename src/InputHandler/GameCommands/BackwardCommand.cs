using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class BackwardCommand : ICommand
	{
		IControllableShip ship;

		public BackwardCommand(IControllableShip s)
		{
			ship = s;
		}

		public void Execute()
		{
			ship.Backward();
		}
	}
}
