using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// base class for entity behaviour when it goes outside of the play area boundaries
	/// </summary>
	public abstract class BoundaryStrategy
	{
		protected Rectangle playArea;

		public BoundaryStrategy(Rectangle playArea) {
			this.playArea = playArea;
		}

		/// <summary>
		/// checks whether entity is in play area or not
		/// </summary>
		/// <param name="entity">entity to check</param>
		/// <returns>true or false</returns>
		protected bool IsInPlay(Entity entity) {
			if (entity != null)
				return entity.RealPos.InRect(playArea);
			else return false;
		}

		public abstract void Run(Entity e);
	}
}
