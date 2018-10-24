using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// The entity will wrap around to the other side if it goes outside of the play area
	/// </summary>
	public class WrapBoundaryBehaviour : BoundaryStrategy
	{
		public WrapBoundaryBehaviour(Rectangle playArea) : base(playArea) { }

		public override void Run(Entity entity) {
			int padding = 10;

			if (!IsInPlay(entity)) {
				Point2D target = entity.RealPos;

				if (entity.RealPos.X < playArea.Left)
					target.X = playArea.Right - padding;
				else if (entity.RealPos.X > playArea.Right)
					target.X = playArea.Left + padding;

				if (entity.RealPos.Y < playArea.Top)
					target.Y = playArea.Bottom - padding;
				else if (entity.RealPos.Y > playArea.Bottom)
					target.Y = playArea.Top + padding;

				entity.TeleportTo(target);
			}
		}
	}
}
