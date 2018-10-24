using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Entities;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.AI.strategies
{
	public class ChaseStrategy : ErraticStrategy
	{
		private IHandlesEntities entHandler;
		private Ship target;
		private float agroRange;

		public ChaseStrategy(IAIEntity controlled, IHandlesEntities entHandler, int shootCooldown) : base(controlled, shootCooldown) {
			this.entHandler = entHandler;
			agroRange = SwinGame.ScreenWidth() / 1.5f;
		}

		protected override void ExecuteStrategy() {
			if (controlled == null || controlled.IsDead)
				return;

			//seek nearest enemy target
			target = FetchNearestTarget();

			//run static strategy if no target found
			if (target == null) {
				base.ExecuteStrategy();
			}
			//chase strategy
			else {
				//get steering vector
				Vector DesiredVec = target.RealPos.Subtract(controlled.RealPos);
				DesiredVec = DesiredVec.LimitToMagnitude(controlled.MaxVel);
				
				Vector SteeringVec = DesiredVec.SubtractVector(controlled.Vel);
				targetDir = SteeringVec.UnitVector;

				controlled.TurnTo(targetDir);

				//thrust
				if (controlled.ShouldThrust(targetDir)) {
					Vector vDir = SwinGame.VectorTo(1, 0);
					controlled.Thrust(vDir);
				}
			}
		}

		/// <summary>
		/// Find nearest entity within agro range
		/// </summary>
		private Ship FetchNearestTarget() {
			Ship result = null;
			float distanceToTarget = agroRange;

			foreach (Ship e in entHandler?.EntityList?.OfType<Ship>()) {
				float distanceToEntity = SwinGame.PointPointDistance(controlled.RealPos, e.RealPos);

				if (e.Team != controlled.Team && distanceToEntity < distanceToTarget) {
					result = e;
					distanceToTarget = distanceToEntity;
				}
			}

			return result;
		}
	}
}
