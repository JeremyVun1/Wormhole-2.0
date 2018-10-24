using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;

namespace TaskForceUltra.src.GameModule
{
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
			if (!cdHandler.OnCooldown()) {
				Spawn();
			}

			if (SwinGame.MouseClicked(MouseButton.LeftButton))
				Spawn();
		}

		private void Spawn() {
			//get random position
			Point2D pos = Util.RandomPointInRect(playArea);
			BoundaryStrategy boundaryStrat = new WrapBoundaryBehaviour(playArea);

			//create ai ship
			Ship aiShip = shipFac.CreateRandomShip(SwinGame.ToWorld(SwinGame.MousePosition()), boundaryStrat, ControllerType.Computer, difficulty, entityHandler);

			//add it to the entity handler
			entityHandler.Track(aiShip);

			cdHandler.StartCooldown();
		}
	}
}