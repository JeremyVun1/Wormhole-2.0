using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Moving and spinning debris that doesn't interact with anything
	/// </summary>
	public class Debris : Mover
	{
		private float friction;
		private float turnRate;
		private CooldownHandler cdHandler;
		public override List<LineSegment> DebrisLines { get { return null; } }

		public Debris(string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int health, Vector vel, Vector dir, float friction, float turnRate,
			float lifetime, BoundaryStrategy boundaryStrat, Team team
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, boundaryStrat, team)
		{
			this.friction = friction;
			this.turnRate = turnRate;

			cdHandler = new CooldownHandler(lifetime * 1000);
			cdHandler.StartCooldown();
		}

		public override void Update() {
			Vel = Vel.Multiply(friction);
			turnRate *= friction;
			theta = turnRate;

			if (!cdHandler.IsOnCooldown())
				Kill(Team.None);

			base.Update();
		}

		public override void Kill(Team killer) {
			base.Kill(Team.None);
		}
	}

	/// <summary>
	/// Creates Debris
	/// </summary>
	public class DebrisFactory
	{
		public Debris Create(LineSegment l, Point2D pos) {
			Shape shape = new Shape(new List<LineSegment> { l }, null, SwinGame.PointAt(0, 0));
			List<Color> colors = new List<Color> { Color.Red };

			Debris result = new Debris("debris", null, pos, SwinGame.PointAt(0, 0), shape, colors,
			1, Util.RandomUnitVector().Multiply(2), Util.RandomUnitVector(), 0.97f, Util.Rand(10), 3, null, Team.Computer);

			result.TeleportTo(pos);

			return result;
		}
	}
}
