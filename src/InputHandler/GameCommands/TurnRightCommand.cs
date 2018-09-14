using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class TurnRightCommand : ICommand
	{
		IControllableShip ship;

		public TurnRightCommand(IControllableShip s)
		{
			ship = s;
		}

		public void Execute()
		{
			ship.TurnRight();
		}
	}
}
