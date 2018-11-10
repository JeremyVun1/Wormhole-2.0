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
	/// chases ship's on the other team if they come within range of it
	/// </summary>
	public class ChaseStrategy : AIStrategy
	{
		private IHandlesEntities entHandler;
		private Ship target;
		private float agroRange;

		public ChaseStrategy(IAIEntity controlled, IHandlesEntities entHandler, int shootCooldown) : base(controlled, shootCooldown) {
			this.entHandler = entHandler;
			agroRange = SwinGame.ScreenWidth() / 1.5f;
		}

		protected override void ExecuteStrategy() {
			base.ExecuteStrategy();

			target = FetchNearestTarget();

			if (target == null) {
				TryThrustForward();
			}
			//chase strategy
			else {
				//steering vector
				Vector DesiredVec = target.RealPos.Subtract(controlled.RealPos);
				DesiredVec = DesiredVec.LimitToMagnitude(controlled.MaxVel);
				Vector SteeringVec = DesiredVec.SubtractVector(controlled.Vel);
				targetDir = SteeringVec.UnitVector;

				TryRotate();
				TryThrustForward();

				//thrust
				/*if (controlled.ShouldThrust(targetDir)) {
					Vector vDir = SwinGame.VectorTo(1, 0);
					controlled.Thrust(vDir);
				}*/
			}
		}

		/// <summary>
		/// Find nearest entity within agro range
		/// </summary>
		/// <returns>target ship</returns>
		private Ship FetchNearestTarget() {
			if (entHandler == null)
				return null;

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
