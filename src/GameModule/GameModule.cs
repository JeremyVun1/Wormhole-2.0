using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Entities;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// The space ship shooting part
	/// </summary>
	public class GameModule {
		public Level Level { get; private set; }

		private EntityHandler entityHandler;
		private CollisionHandler collisionHandler;
		private CameraHandler cameraHandler;
		private InputController inputController;
		private AISpawner aiSpawner;
		private Ship player;

		private Scoresheet scoresheet;
		private GameSendData gameSendData;
		private Timer gameTimer;

		public GameModule(Ship p, Level level, AISpawner aiSpawner, EntityHandler entHandler, CollisionHandler collHandler,
			CameraHandler camHandler, GameSendData gameSendData, Scoresheet scoresheet, InputController inpController
		)
		{
			player = p;
			Level = level;
			entityHandler = entHandler;
			collisionHandler = collHandler;
			cameraHandler = camHandler;
			inputController = inpController;
			this.scoresheet = scoresheet;
			this.aiSpawner = aiSpawner;
			this.gameSendData = gameSendData;

			gameTimer = SwinGame.CreateTimer();
			gameTimer.Start();
		}

		public void Update() {
			Level.Update();
			entityHandler.Update();
			collisionHandler.Update();
			cameraHandler.Update();
			inputController.Update();
			aiSpawner.Update();

			if (IsGameEnd())
				EndGame();
		}

		private bool IsGameEnd() {
			if (player.IsDead || entityHandler.AIShipCount() == 0)
				return true;
			return false;
		}

		public void Draw() {
			Level.Draw();
			entityHandler.Draw(cameraHandler.Viewport);

			//draw points
			Rectangle scoreRect = SwinGame.CreateRectangle(0, 0, SwinGame.ScreenWidth(), SwinGame.ScreenHeight()*0.1f);
			SwinGame.DrawText(scoresheet.FetchTeamScore(Team.Team1).ToString(), Color.Yellow, Color.Transparent, "MenuTitle", FontAlignment.AlignCenter, scoreRect);
		}

		/// <summary>
		/// End the game module and send game results to game controller
		/// </summary>
		public void EndGame() {
			gameSendData.Add(GameResultType.Points, scoresheet.FetchTeamScore(player.Team));
			gameSendData.Add(GameResultType.Result, player.IsDead ? 0 : 1);
			gameSendData.Add(GameResultType.Time, (int)gameTimer.Ticks);

			SwinGame.FreeTimer(gameTimer);

			gameSendData.Send();
		}
	}

	/// <summary>
	/// Game Module Factory
	/// </summary>
	public class GameModuleFactory
	{
		public GameModule Create(string shipId, Difficulty diff, Level level, ShipFactory shipFac, GameSendData gameSendData) {
			Scoresheet scoreSheet = new Scoresheet();
			EntityHandler entHandler = new EntityHandler(scoreSheet, level.PlayArea);
			CollisionHandler collHandler = new CollisionHandler(level.PlayArea, entHandler);

			Ship p = shipFac.Create(shipId, Util.RandomPointInRect(level.PlayArea), new WrapBoundaryBehaviour(level.PlayArea), ControllerType.Player1, diff, entHandler);
			entHandler.Track(p);

			CameraHandler camHandler = new CameraHandler(p, level.PlayArea);
			AISpawner aiSpawner = new AISpawner(diff, level.PlayArea, shipFac, entHandler);

			//spawn predefined ships from the level
			foreach(string enemyId in level.ShipsToSpawn) {
				Ship toSpawn = shipFac.Create(enemyId, Util.RandomPointInRect(level.PlayArea), new WrapBoundaryBehaviour(level.PlayArea), ControllerType.Computer, diff, entHandler);
				entHandler.Track(toSpawn);
			}

			//spawn asteroids
			AsteroidFactory asteroidFac = new AsteroidFactory();
			string asteroidPath = SwinGame.AppPath() + "\\resources\\data\\asteroids\\asteroid.json";
			for (int i=0; i<level.AsteroidsToSpawn; i++) {
				Asteroid toSpawn = asteroidFac.Create(asteroidPath, level.PlayArea);
				entHandler.Track(toSpawn);
			}

			InputController inpController;
			if (p is IControllable) {
				InputControllerFactory inpContFac = new InputControllerFactory();
				inpController = inpContFac.Create(p as IControllable, ControllerType.Player1);
			}
			else {
				inpController = null;
			}

			return new GameModule(p, level, aiSpawner, entHandler, collHandler, camHandler, gameSendData, scoreSheet, inpController);
		}
	}

}
