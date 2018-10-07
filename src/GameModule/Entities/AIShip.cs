using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.AI;

namespace TaskForceUltra.src.GameModule.Entities
{
	public class AIShip : Ship, IAIEntity
	{
		private AIStrategy aiStrategy;

		public AIStrategy AIStrategy { set { aiStrategy = value; } }

		public AIShip(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int health, Vector vel, Vector dir, BoundaryStrategy boundaryStrat, Team team,
			List<Component> components
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, 500, boundaryStrat, team, components)
		{

		}

		public override void Update() {
			if (aiStrategy != null) {
				aiStrategy.Update();
				base.Update();
			}
		}
	}
}
