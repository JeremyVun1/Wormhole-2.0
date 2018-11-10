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
		private CommandHistory commandHistory;

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
			commandHistory = new CommandHistory(200);
		}

		public void Update() {
			if (Util.IsUndoMode)
				UndoCommands();
			else HandleInput();

			DrawUI();
		}

		private void DrawUI() {
			float w = SwinGame.ScreenWidth();
			float h = SwinGame.ScreenHeight();
			Rectangle rect = SwinGame.CreateRectangle(SwinGame.ToWorldX(w*0.3f), SwinGame.ToWorldY(h*0.92f), w*0.4f, h*0.02f);

			SwinGame.FillRectangle(Color.Red, rect.X, rect.Y, rect.Width * commandHistory.Count / 200, rect.Height);
			SwinGame.DrawRectangle(Color.White, rect);
		}

		private void UndoCommands() {
			if (commandHistory.HasSteps()) {
				DrawOverlay();
				commandHistory.UndoLastStep();
			}
			else Util.IsUndoMode = false;
		}

		private void HandleInput() {
			//activate time reverse
			if (bindings.ReverseTime()) {
				Util.IsUndoMode = true;
				return;
			}

			//Movement
			if (bindings.Forward()) {
				forwardCommand.Execute();
				commandHistory.AddCommand(forwardCommand);
			}				
			if (bindings.Backward()) {
				backwardsCommand.Execute();
				commandHistory.AddCommand(backwardsCommand);
			}				
			if (bindings.StrafeLeft()) {
				strafeLeftCommand.Execute();
				commandHistory.AddCommand(strafeLeftCommand);
			}

			if (bindings.StrafeRight()) {
				strafeRightcommand.Execute();
				commandHistory.AddCommand(strafeRightcommand);
			}

			//Rotation
			if (bindings.TurnRight()) {
				turnRightCommand.Execute();
				commandHistory.AddCommand(turnRightCommand);
			}

			if (bindings.TurnLeft()) {
				turnLeftCommand.Execute();
				commandHistory.AddCommand(turnLeftCommand);
			}

			//actions
			if (bindings.Shoot()) {
				shootCommand.Execute();
				commandHistory.AddCommand(shootCommand);
			}
				
			if (bindings.ActivatePowerup()) {
				Console.WriteLine("activate powerup input received");
			}

			//go to next command step - allow multiple commmands to be stored in the same step and undone at the same time.
			commandHistory.NextStep();
		}

		private void DrawOverlay() {
			Rectangle screenRect = SwinGame.CreateRectangle(Camera.CameraPos().X, Camera.CameraPos().Y, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
			SwinGame.FillRectangle(SwinGame.RGBAColor(255, 0, 0, 80), screenRect);
			Rectangle textRect = SwinGame.CreateRectangle(0, SwinGame.ScreenHeight() * 0.85f, SwinGame.ScreenWidth(), 100);
			SwinGame.DrawText("UNDOING COMMANDS", Color.White, Color.Transparent, "MenuTitle", FontAlignment.AlignCenter, textRect);
		}

		private void CreateCommands() {
			GameCommandFactory commandFac = new GameCommandFactory(controlled);

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
