using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.AI;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// AI ship that utilitises an AI strategy
	/// </summary>
	public class AIShip : Ship, IAIEntity
	{
		public AIStrategy AIStrategy { private get; set; }

		public AIShip(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int health, Vector vel, Vector dir, BoundaryStrategy boundaryStrat, Team team,
			List<Component> components
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, 500, boundaryStrat, team, components)
		{

		}

		public override void Update() {
			if (AIStrategy != null) {
				AIStrategy.Update();
				base.Update();
			}
		}
	}
}
