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
		private List<ICommand> commandHistory;

		private ICommand forwardCommand;
		private ICommand backwardsCommand;
		private ICommand strafeLeftCommand;
		private ICommand strafeRightcommand;
		private ICommand turnLeftCommand;
		private ICommand turnRightCommand;
		private ICommand activatePowerupCommand;
		private ICommand shootCommand;

		private bool IsUndoMode;
		private int i;

		public InputController(IControllable c, IActionBinding b)
		{
			controlled = c;
			bindings = b;

			CreateCommands();
			IsUndoMode = false;
			commandHistory = new List<ICommand>();
		}

		public void Update() {
			if (IsUndoMode)
				UndoCommand();
			else HandleInput();

			TrimCommandHistory();
		}

		private void TrimCommandHistory() {
			while (commandHistory.Count > 300) {
				commandHistory.Reverse();
				commandHistory.RemoveAt((commandHistory.Count - 1));
				commandHistory.Reverse();
			}
		}

		private void UndoCommand() {
			if (i < 0) {
				IsUndoMode = false;
				commandHistory.Clear();
			}
			else {
				commandHistory[i].Undo();
				Rectangle screenRect = SwinGame.CreateRectangle(Camera.CameraPos().X, Camera.CameraPos().Y, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
				SwinGame.FillRectangle(SwinGame.RGBAColor(255, 0, 0, 80), screenRect);
				Rectangle textRect = SwinGame.CreateRectangle(0, SwinGame.ScreenHeight() * 0.85f, SwinGame.ScreenWidth(), 100);
				SwinGame.DrawText("UNDOING COMMANDS", Color.White, Color.Transparent, "MenuTitle", FontAlignment.AlignCenter, textRect);

				i -= 1;
			}
		}

		private void HandleInput() {
			//super power reverse actions
			if (bindings.ReverseTime()) {
				IsUndoMode = true;
				i = commandHistory.Count-1;
				return;
			}

			//Movement
			if (bindings.Forward()) {
				forwardCommand.Execute();
				commandHistory.Add(forwardCommand);
			}				
			if (bindings.Backward()) {
				backwardsCommand.Execute();
				commandHistory.Add(backwardsCommand);
			}				
			if (bindings.StrafeLeft()) {
				strafeLeftCommand.Execute();
				commandHistory.Add(strafeLeftCommand);
			}

			if (bindings.StrafeRight()) {
				strafeRightcommand.Execute();
				commandHistory.Add(strafeRightcommand);
			}

			//Rotation
			if (bindings.TurnRight()) {
				turnRightCommand.Execute();
				commandHistory.Add(turnRightCommand);
			}

			if (bindings.TurnLeft()) {
				turnLeftCommand.Execute();
				commandHistory.Add(turnLeftCommand);
			}

			//actions
			if (bindings.Shoot()) {
				shootCommand.Execute();
				commandHistory.Add(shootCommand);
			}
				
			if (bindings.ActivatePowerup()) {
				Console.WriteLine("activate powerup input received");
			}
				
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
