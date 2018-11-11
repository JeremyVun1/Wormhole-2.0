using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.AI.strategies
{
	/// <summary>
	/// Keep going forward. even dumber than static strategy
	/// </summary>
	public class ForwardStrategy : AIStrategy
	{
		public ForwardStrategy(IAIEntity controlled, int shootCd = 0) : base(controlled, shootCd) {
		}

		/// <summary>
		/// Keep turning left or right based on random number on initialisation
		/// </summary>
		protected override void ExecuteStrategy() {
			base.ExecuteStrategy();

			ThrustForward();
		}
	}
}
