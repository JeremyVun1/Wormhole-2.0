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
	public class PlayerShip : Ship
	{
		public PlayerShip(
			string id, string filePath, Point2D refPos, Point2D offsetPos,
			Shape shape, List<Color> colors, int health, Vector vel, Vector dir,
			BoundaryStrategy boundaryStrat, Team team, List<Component> components
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, 2000, boundaryStrat, team, components)
		{
		}

		public override bool TryReactToCollision(int dmg, Vector collidingVel, int collidingMass, Team collider, bool forceReaction = false) {
			forceReaction = false;
			return base.TryReactToCollision(dmg, collidingVel, collidingMass, collider, forceReaction);
		}
	}
}
