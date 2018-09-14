using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public interface ICollidable
	{
		Rectangle BoundingBox { get; }

		bool CollidingWith(ICollidable other);

		void ReactToCollision(int dmg, Point2D vel, Vector dir);
	}
}
