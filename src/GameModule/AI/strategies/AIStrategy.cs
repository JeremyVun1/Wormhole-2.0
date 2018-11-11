using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Entities;
using TaskForceUltra.src.GameModule.AI.strategies;
using TaskForceUltra.src.GameModule.Commands;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.AI
{
	/// <summary>
	/// base class for AI strategy
	/// </summary>
	public abstract class AIStrategy
	{
		protected IAIEntity controlled;
		protected Vector targetDir;
		protected CooldownHandler shootCooldown;
		protected InputController inputController;
		protected CommandHistory commandHistory;

		protected ICommand forwardCommand;
		protected ICommand backwardsCommand;
		protected ICommand strafeLeftCommand;
		protected ICommand strafeRightcommand;
		protected ICommand turnLeftCommand;
		protected ICommand turnRightCommand;
		protected ICommand shootCommand;

		public AIStrategy(IAIEntity controlled, int shootCd = 0) {
			this.controlled = controlled;
			targetDir = Util.RandomUnitVector();

			commandHistory = new CommandHistory(200);
			CreateCommands();

			if (shootCd > 0)
				shootCooldown = new CooldownHandler(shootCd * 1000);
		}

		public void Update() {
			if (controlled.IsDead)
				return;

			if (Util.IsUndoMode)
				UndoCommands();
			else {
				Shoot();
				ExecuteStrategy();
				commandHistory.NextStep();
			}
		}

		private void UndoCommands() {
			if (commandHistory.HasSteps()) {
				commandHistory.UndoLastStep();
			}
		}

		protected virtual void ExecuteStrategy() {
			if (controlled == null || controlled.IsDead)
				return;

			if (SwinGame.KeyTyped(KeyCode.GKey)) {
				Util.IsUndoMode = true;
				return;
			}
		}

		protected void Shoot() {
			//guard
			if (shootCooldown == null)
				return;

			if (!shootCooldown.IsOnCooldown()) {
				controlled.Fire();
				shootCooldown.StartCooldown();
			}
		}

		protected void ThrustForward() {
			forwardCommand.Execute();
			commandHistory.AddCommand(forwardCommand);
		}

		protected void TryRotate() {
			if (SwinGame.CalculateAngle(controlled.Dir, targetDir).GetSign() > 0) {
				turnRightCommand.Execute();
				commandHistory.AddCommand(turnRightCommand);
			}
			else {
				turnLeftCommand.Execute();
				commandHistory.AddCommand(turnLeftCommand);
			}
		}

		private void CreateCommands() {
			var commandFac = new GameCommandFactory(controlled);

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
	/// returns a randomised ai strategy based on some probability curve from a difficulty level
	/// </summary>	
	public class AIStrategyFactory
	{
		private int difficultyLevel;
		private int shootCooldown;

		public AIStrategyFactory(int difficultyLevel, int shootCooldown) {
			this.difficultyLevel = difficultyLevel;
			this.shootCooldown = shootCooldown;
		}

		public AIStrategy Create(IAIEntity aiEntity, IHandlesEntities entHandler) {
			//generate random number up to the difficulty level
			int n = Util.Rand(difficultyLevel);

			//return the hardest strategy that the number can get
			if (n < 5) {
				return new CrazyRotatingStrategy(aiEntity, shootCooldown);
			}
			else if (n < 10) {
				return new StaticStrategy(aiEntity, shootCooldown);
			}
			else if (n < 20) {
				return new ErraticStrategy(aiEntity, shootCooldown);
			}
			else {
				return new ChaseStrategy(aiEntity, entHandler, shootCooldown);
			}
		}

		public AIStrategy CreateByName(string strategy, IAIEntity aiEntity, IHandlesEntities entHandler) {
			switch (strategy.ToLower()) {
				case "crazyrotating":
					return new CrazyRotatingStrategy(aiEntity, shootCooldown);
				case "chase":
					return new ChaseStrategy(aiEntity, entHandler, shootCooldown);
				case "erratic":
					return new ErraticStrategy(aiEntity, shootCooldown);
				case "static":
					return new StaticStrategy(aiEntity, shootCooldown);
				case "forward":
					return new ForwardStrategy(aiEntity, shootCooldown);
				case "spreadforward":
					return new SpreadForwardStrategy(aiEntity, shootCooldown);
				default:
					return new StaticStrategy(aiEntity, shootCooldown);
			}
		}

		public AIStrategy Create(IAIEntity aiEntity) {
			return Create(aiEntity, null);
		}
	}
}
