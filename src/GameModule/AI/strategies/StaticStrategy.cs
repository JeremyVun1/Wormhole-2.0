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
	/// Choose a direction once and keep thrusting in that direction until we die
	/// </summary>
	public class StaticStrategy : AIStrategy
	{
		public StaticStrategy(IAIEntity controlled, int shootCooldown) : base(controlled, shootCooldown) {
			targetDir = Util.RandomUnitVector();
		}

		protected override void ExecuteStrategy() {
			base.ExecuteStrategy();

			TryRotate();
			TryThrustForward();
		}
	}
}
