using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class TurnLeftCommand : ICommand
	{
		IControllableShip ship;

		public TurnLeftCommand(IControllableShip s)
		{
			ship = s;
		}

		public void Execute()
		{
			ship.TurnLeft();
		}
	}
}
