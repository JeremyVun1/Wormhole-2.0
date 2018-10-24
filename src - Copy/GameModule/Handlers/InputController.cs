using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Commands;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Invoker object that listens for player input on the passed in action bindings
	/// and activates the relevant commands
	/// </summary>
	public class InputController
	{
		private IControllable controlled;
		private IActionBinding bindings;

		private ICommand forwardCommand;
		private ICommand backwardsCommand;
		private ICommand strafeLeftCommand;
		private ICommand strafeRightcommand;
		private ICommand turnLeftCommand;
		private ICommand turnRightCommand;
		private ICommand activatePowerupCommand;
		private ICommand shootCommand;

		public InputController(IControllable c, IActionBinding b)
		{
			controlled = c;
			bindings = b;

			CreateCommands();
		}

		public void Update() {
			HandleInput();
		}

		private void HandleInput() {
			//Movement
			if (bindings.Forward())
				forwardCommand.Execute();
			if (bindings.Backward())
				backwardsCommand.Execute();
			if (bindings.StrafeLeft())
				strafeLeftCommand.Execute();
			if (bindings.StrafeRight())
				strafeRightcommand.Execute();

			//Rotation
			if (bindings.TurnRight())
				turnRightCommand.Execute();
			if (bindings.TurnLeft())
				turnLeftCommand.Execute();

			//actions
			if (bindings.Shoot())
				shootCommand.Execute();
			if (bindings.ActivatePowerup())
				Console.WriteLine("activate powerup input received");
		}

		private void CreateCommands() {
			var commandFac = new GameCommandFactory(controlled);

			activatePowerupCommand = commandFac.Create(ShipAction.ActivatePowerup);
			shootCommand = commandFac.Create(ShipAction.Shoot);
			forwardCommand = commandFac.Create(ShipAction.Forward);
			backwardsCommand = commandFac.Create(ShipAction.Backward);
			strafeLeftCommand = commandFac.Create(ShipAction.StrafeLeft);
			strafeRightcommand = commandFac.Create(ShipAction.StrafeRight);
			turnLeftCommand = commandFac.Create(ShipAction.TurnLeft);
			turnRightCommand = commandFac.Create(ShipAction.TurnRight);
		}
	}


	/// <summary>
	/// Input Controller Factory
	/// </summary>
	public class InputControllerFactory
	{
		public InputController Create(IControllable controlled, ControllerType controller) {
			ActionBindingFactory actionBindingFac = new ActionBindingFactory(SwinGame.AppPath() + "\\resources\\data\\bindings");

			switch(controller) {
				case ControllerType.Player1:
					return new InputController(controlled, actionBindingFac.Create(ControllerType.Player1));
				case ControllerType.Player2:
					return new InputController(controlled, actionBindingFac.Create(ControllerType.Player2));
				case ControllerType.Player3:
					return new InputController(controlled, actionBindingFac.Create(ControllerType.Player3));
				case ControllerType.Player4:
					return new InputController(controlled, actionBindingFac.Create(ControllerType.Player4));
				default:
					return new InputController(controlled, actionBindingFac.Create(ControllerType.Player1));
			}
		}
	}
}
