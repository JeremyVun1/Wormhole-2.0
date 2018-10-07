using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Entities;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.AI.strategies
{
	/// <summary>
	/// randomly change our direction about every 5 seconds
	/// </summary>
	public class ErraticStrategy : AIStrategy
	{
		private CooldownHandler cdHandler;
		private int turnCooldown;

		public ErraticStrategy(IAIEntity controlled, int shootCooldown) : base(controlled, shootCooldown) {
			turnCooldown = 5000;

			cdHandler = new CooldownHandler(Util.Rand(turnCooldown));
			cdHandler.StartCooldown();
		}

		protected override void ExecuteStrategy() {
			base.ExecuteStrategy();

			//set new random vector and random timer threshhold
			if (!cdHandler.OnCooldown()) {
				Vector newDir = Util.RandomUnitVector();
				cdHandler.StartNewThreshhold(Util.Rand(turnCooldown));
			}

			//rotate
			controlled.TurnTo(targetDir);

			//thrust
			if (controlled.ShouldThrust(targetDir)) {
				Vector vDir = SwinGame.VectorTo(1, 0);
				controlled.Thrust(vDir);
			}
		}
	}
}
