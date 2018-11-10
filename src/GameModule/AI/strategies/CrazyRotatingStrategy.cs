using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.AI.strategies
{
	/// <summary>
	/// Keep rotating
	/// </summary>
	public class CrazyRotatingStrategy : AIStrategy
	{
		private int rotationDir;

		public CrazyRotatingStrategy(IAIEntity controlled) : base(controlled) {
			rotationDir = Util.Rand(-1, 2);
		}

		/// <summary>
		/// Keep turning left or right based on random number on initialisation
		/// </summary>
		protected override void ExecuteStrategy() {
			base.ExecuteStrategy();

			if (rotationDir <= 0) {
				turnLeftCommand.Execute();
				commandHistory.AddCommand(turnLeftCommand);
			}
			else if (rotationDir > 0) {
				turnRightCommand.Execute();
				commandHistory.AddCommand(turnRightCommand);
			}
		}
	}
}
