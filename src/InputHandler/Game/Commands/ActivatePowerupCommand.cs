using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class ActivatePowerupCommand : ICommand
	{
		IControllableShip ship;

		public ActivatePowerupCommand(IControllableShip s)
		{
			ship = s;
		}

		public void Execute()
		{
			ship.ActivatePowerup();
		}
	}
}
