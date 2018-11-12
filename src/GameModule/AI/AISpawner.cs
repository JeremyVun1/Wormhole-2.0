using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// handles spawning of AI ships based on a cooldown modified by difficulty
	/// </summary>
	public class AISpawner
	{
		private IHandlesEntities entityHandler;
		private CooldownHandler cdHandler;
		private ShipFactory shipFac;
		private Difficulty difficulty;
		private Rectangle playArea;

		public AISpawner(Difficulty diff, Rectangle playArea, ShipFactory shipFactory, IHandlesEntities entHandler) {
			entityHandler = entHandler;
			shipFac = shipFactory;
			difficulty = diff;

			this.playArea = playArea;

			cdHandler = new CooldownHandler(diff.SpawnTimer * 1000);
			cdHandler.StartCooldown();
		}

		public virtual void Update() {
			if (!cdHandler.IsOnCooldown() && entityHandler.EntityList.Count < 5) {
				Spawn();
			}

			if (SwinGame.MouseClicked(MouseButton.LeftButton))
				Spawn();
		}

		/// <summary>
		/// Spawn a random AI ship
		/// </summary>
		private void Spawn() {
			Point2D pos = Util.RandomPointInRect(playArea);
			BoundaryStrategy boundaryStrat = new WrapBoundaryBehaviour(playArea);

			Ship aiShip = shipFac.CreateRandomShip(SwinGame.ToWorld(SwinGame.MousePosition()), boundaryStrat, ControllerType.Computer, difficulty, entityHandler);
			entityHandler.Track(aiShip);

			cdHandler.StartCooldown();
		}
	}
}