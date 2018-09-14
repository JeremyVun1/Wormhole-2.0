using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	//invoker for controlling IControllableShip objects
	public class GameInputHandler
	{
		//control interfaces
		private IControllableShip activeShip;
		private IActionBinding bindings;
		private GameCommandFactory commandFac;

		//command objects
		private ICommand activatePowerupCommand;
		private ICommand shootCommand;
		private ICommand forwardCommand;
		private ICommand backwardsCommand;
		private ICommand strafeLeftCommand;
		private ICommand strafeRightcommand;
		private ICommand turnLeftCommand;
		private ICommand turnRightCommand;

		public GameInputHandler(IControllableShip s, IActionBinding b)
		{
			activeShip = s;
			bindings = b;

			commandFac = new GameCommandFactory(activeShip);

			CreateCommands();
		}

		private void CreateCommands()
		{
			activatePowerupCommand = commandFac.Create(ShipAction.ActivatePowerup);
			shootCommand = commandFac.Create(ShipAction.Shoot);
			forwardCommand = commandFac.Create(ShipAction.Forward);
			backwardsCommand = commandFac.Create(ShipAction.Backward);
			strafeLeftCommand = commandFac.Create(ShipAction.StrafeLeft);
			strafeRightcommand = commandFac.Create(ShipAction.StrafeRight);
			turnLeftCommand = commandFac.Create(ShipAction.TurnLeft);
			turnRightCommand = commandFac.Create(ShipAction.TurnRight);
		}

		public void SetActiveShip(IControllableShip s)
		{
			activeShip = s;
		}

		public void Update()
		{
			HandleInput();
		}

		private void HandleInput()
		{
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
				Log.Msg("activate powerup input received");
		}
	}
}
