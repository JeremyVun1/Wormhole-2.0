using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class GameCommandFactory
	{
		private IControllable controlled;
		
		public GameCommandFactory(IControllable controlled) {
			this.controlled = controlled;
		}

		public ICommand Create(ShipAction action) {
			switch (action) {
				case ShipAction.Forward:
					return new ForwardCommand(controlled);
				case ShipAction.Backward:
					return new BackwardCommand(controlled);
				case ShipAction.StrafeLeft:
					return new StrafeLeftCommand(controlled);
				case ShipAction.StrafeRight:
					return new StrafeRightCommand(controlled);
				case ShipAction.TurnLeft:
					return new TurnLeftCommand(controlled);
				case ShipAction.TurnRight:
					return new TurnRightCommand(controlled);
				case ShipAction.ActivatePowerup:
					return new ActivatePowerupCommand(controlled);
				case ShipAction.Shoot:
					return new ShootCommand(controlled);
				default:
					return null;
			}
		}
	}
}
