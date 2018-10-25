using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	public class NumberPopup : Mover
	{
		private string text;
		private CooldownHandler cdHandler;

		public NumberPopup(string id, int points, Point2D refPos, List<Color> colors, float vel,
			Vector dir, float lifetime, BoundaryStrategy boundaryStrat, Team team, bool optimiseMe
		) : base(id, null, refPos, SwinGame.PointAt(0,0), null, colors, 0, dir.Multiply(vel), dir, boundaryStrat, team, optimiseMe)
		{
			text = $"+{points}";
			cdHandler = new CooldownHandler(lifetime * 1000);
			cdHandler.StartCooldown();
		}

		public override void Draw() {
			SwinGame.DrawText(text, colors[colorIndex], "PopupText", refPos.X, refPos.Y);
		}

		public override void Update() {
			if (!cdHandler.IsOnCooldown()) {
				Kill(Team.None);
			}
			base.Update();
		}
	}

	public class NumberPopupFactory
	{
		private Rectangle playArea;

		public NumberPopupFactory(Rectangle playArea) {
			this.playArea = playArea;
		}

		public NumberPopup Create(Point2D pos, int points) {
			Point2D offset = SwinGame.PointAt(Util.Rand(30) - 15, Util.Rand(30) - 15);
			pos = pos.Add(offset);

			List<Color> colors = new List<Color>() { Color.Yellow };
			float vel = Util.Rand(1000, 3000) / 1000f;

			float x = Util.Rand(500, 2000) / 8000f;
			Vector dir = SwinGame.VectorTo(x, -1);
			float lifetime = 1.5f;
			BoundaryStrategy boundaryStrat = new DieBoundaryBehaviour(playArea);

			return new NumberPopup(points.ToString(), points, pos, colors, vel, dir, lifetime, boundaryStrat, Team.None, true);
		}
	}
}
