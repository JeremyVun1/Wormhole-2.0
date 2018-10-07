using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	public abstract class BoundaryStrategy
	{
		protected Rectangle playArea;

		public BoundaryStrategy(Rectangle playArea) {
			this.playArea = playArea;
		}

		//check whether entity is in play area or not
		protected bool InPlay(Entity entity) {
			if (entity != null)
				return entity.RealPos.InRect(playArea);
			else return false;
		}

		public abstract void Run(Entity e);
	}
}
