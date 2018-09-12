using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class ForwardCommand : ICommand
	{
		IControllableShip ship;

		public ForwardCommand(IControllableShip s)
		{
			ship = s;
		}

		public void Execute()
		{
			ship.Forward();
		}
	}
}
