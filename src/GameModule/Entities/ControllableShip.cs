using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Player ship that can be controlled by an input controller
	/// </summary>
	public class ControllableShip : Ship, IControllable
	{
		public ControllableShip(
			string id, string filePath, Point2D refPos, Point2D offsetPos,
			Shape shape, List<Color> colors, int health, Vector vel, Vector dir, int hurtThreshhold,
			BoundaryStrategy boundaryStrat, Team team, List<Component> components
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, hurtThreshhold, boundaryStrat, team, components)
		{
		}

		public override bool TryReactToCollision(int dmg, Vector collidingVel, int collidingMass, Team collider, bool forceReaction = false) {
			forceReaction = false;
			return base.TryReactToCollision(dmg, collidingVel, collidingMass, collider, forceReaction);
		}

		/// <summary>
		/// receiver command implementations
		/// </summary>
		public void ForwardCommand() {
			Thrust(SwinGame.VectorTo(1, 0));
		}

		public void BackwardCommand() {
			Thrust(SwinGame.VectorTo(-1, 0));
		}

		public void StrafeLeftCommand() {
			Thrust(SwinGame.VectorTo(0, -1));
		}

		public void StrafeRightCommand() {
			Thrust(SwinGame.VectorTo(0, 1));
		}

		public void TurnLeftCommand() {
			Turn(-1);
		}

		public void TurnRightCommand() {
			Turn(1);
		}

		public void ActivatePowerupCommand() {
			//TODO need playership inventory system
		}

		public void ShootCommand() {
			Fire();
		}
	}
}
