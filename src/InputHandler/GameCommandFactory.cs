using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class GameCommandFactory
	{
		IControllableShip ActiveShip;

		public GameCommandFactory(IControllableShip s)
		{
			ActiveShip = s;
		}

		public ICommand Create(ShipAction action)
		{
			switch (action)
			{
				case ShipAction.ActivatePowerup:
					return new ActivatePowerupCommand(ActiveShip);
				case ShipAction.Backward:
					return new BackwardCommand(ActiveShip);
				case ShipAction.Forward:
					return new ForwardCommand(ActiveShip);
				case ShipAction.Shoot:
					return new ShootCommand(ActiveShip);
				case ShipAction.StrafeLeft:
					return new StrafeLeftCommand(ActiveShip);
				case ShipAction.StrafeRight:
					return new StrafeRightCommand(ActiveShip);
				case ShipAction.TurnLeft:
					return new TurnLeftCommand(ActiveShip);
				case ShipAction.TurnRight:
					return new TurnRightCommand(ActiveShip);
				default:
					return null;
			}
		}
	}
}
