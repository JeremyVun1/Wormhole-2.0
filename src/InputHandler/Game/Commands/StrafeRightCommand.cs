using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class StrafeRightCommand : ICommand
	{
		IControllableShip ship;
		
		public StrafeRightCommand(IControllableShip s)
		{
			ship = s;
		}

		public void Execute()
		{
			ship.StrafeRight();
		}
	}
}
