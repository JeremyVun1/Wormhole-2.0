using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule
{
	public interface ICollides {
		Point2D RealPos { get; }
		Team Team { get; }
		Vector Vel { get; }
		int Mass { get; }
		int Damage { get; }
		List<LineSegment> BoundingBox { get; }

		bool TryReactToCollision(int dmg, Vector collidingVel, int collidingMass, Team collider, bool forceReaction = false);
	}
}
