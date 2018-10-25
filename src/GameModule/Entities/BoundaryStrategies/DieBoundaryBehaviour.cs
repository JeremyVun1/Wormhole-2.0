using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// The entity will die if it goes outside of the play area
	/// </summary>
	public class DieBoundaryBehaviour : BoundaryStrategy
	{
		public DieBoundaryBehaviour(Rectangle playArea) : base(playArea) { }

		public override void Run(Entity entity) {
			if (!IsInPlay(entity)) {
				entity.Kill(Team.None);
			}
		}
	}
}
