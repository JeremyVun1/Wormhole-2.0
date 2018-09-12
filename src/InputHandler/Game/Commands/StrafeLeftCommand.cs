using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class StrafeLeftCommand : ICommand
	{
		IControllableShip ship;

		public StrafeLeftCommand(IControllableShip s)
		{
			ship = s;
		}

		public void Execute()
		{
			ship.StrafeLeft();
		}
	}
}
