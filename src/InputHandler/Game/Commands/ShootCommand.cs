using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class ShootCommand : ICommand
	{
		IControllableShip ship;

		public ShootCommand(IControllableShip s)
		{
			ship = s;
		}

		public void Execute()
		{
			ship.Shoot();
		}
	}
}
