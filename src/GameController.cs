using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using SwinGameSDK;
using TaskForceUltra.src.GameModule;
using TaskForceUltra.src.MenuModule;

namespace TaskForceUltra
{
	public class GameController : IModuleInterface
	{
		private readonly string resourcePath;
		private Color windowColor;

		//Modules
		private MenuModule menuModule;
		private GameModule gameModule;

		//State
		private Bank bank;
		private enum State { MENU, GAME, EXIT };
		private enum Trigger { TOGGLE, EXIT };
		private StateMachine<State, Trigger> stateMachine;

		public GameController(string title, int width, int height, Color color)
		{
			SwinGame.OpenGraphicsWindow(title, width, height);
			windowColor = color;
			resourcePath = SwinGame.AppPath() + "\\Resources";

			Init();
		}
		public GameController() : this("Task Force Ultra", 900, 700, Color.Black) { }

		private void ConfigureStateMachine() {
			stateMachine.Configure(State.MENU)
				.Permit(Trigger.TOGGLE, State.GAME)
				.Permit(Trigger.EXIT, State.EXIT);
			stateMachine.Configure(State.GAME)
				.Permit(Trigger.TOGGLE, State.MENU)
				.Permit(Trigger.EXIT, State.EXIT);
		}

		private void Init() {
			//audio and resources
			SwinGame.OpenAudio();
			SwinGame.SetMusicVolume(0.1f);
			SwinGame.LoadResourceBundleNamed("GameBundle", "Game_Bundle.txt", true);
			SwinGame.PlayMusic("GameMusic");

			//player bank
			bank = new Bank(resourcePath + "\\data\\progress.json");

			//create MenuModule
			ShipFactory shipFac = new ShipFactory(resourcePath + "\\data\\ships");
			LevelFactory levelFac = new LevelFactory(resourcePath + "\\data\\levels");
			MenuModuleFactory menuModuleFac = new MenuModuleFactory(resourcePath + "\\data\\menus");
			menuModule = menuModuleFac.Create(bank, shipFac.ShapeRegistry, levelFac.levelList, this, Exit);

			//state machine
			stateMachine = new StateMachine<State, Trigger>(State.MENU);
			ConfigureStateMachine();
		}

		/// <summary>
		/// run the game
		/// </summary>
		public void Run() {
			while (!ExitRequested()) {
				SwinGame.ClearScreen(windowColor);
				SwinGame.ProcessEvents();

				Update();
				Draw();

				SwinGame.DrawFramerate(0, 0);
				SwinGame.RefreshScreen(60);
			}
		}

		private void Update() {
			DebugMode.HandleInput();

			switch (stateMachine.State) {
				case (State.GAME):
					gameModule.Update();
					break;
				case (State.MENU):
					menuModule.Update();
					break;
			}
		}

		private void Draw() {
			switch (stateMachine.State) {
				case (State.GAME):
					gameModule.Draw();
					break;
				case (State.MENU):
					menuModule.Draw();
					break;
			}
		}

		private bool ExitRequested() {
			return (SwinGame.WindowCloseRequested() || stateMachine.State == State.EXIT);
		}

		public void Exit() {
			stateMachine.Fire(Trigger.EXIT);
		}

		/// <summary>
		/// receives data from the menu module and starts the game module
		/// </summary>
		/// <param name="receiveData">Selected Game Options</param>
		public void ReceiveMenuData(Dictionary<SelectionType, string> receiveData) {
			LevelFactory levelFac = new LevelFactory(resourcePath + "\\data\\levels");
			ShipFactory shipFac = new ShipFactory(resourcePath + "\\data\\ships");
			GameModuleFactory gameModuleFac = new GameModuleFactory();

			Difficulty diff = DifficultySetting.Fetch(receiveData[SelectionType.Difficulty]);
			Level level = levelFac.Fetch(receiveData[SelectionType.Level]);
			string shipId = receiveData[SelectionType.Ship];

			GameSendData gameSendData = new GameSendData(this);
			
			gameModule = gameModuleFac.Create(shipId, diff, level, shipFac, gameSendData);
			stateMachine.Fire(Trigger.TOGGLE);
		}

		/// <summary>
		/// Receives data from the game module and starts the menu module
		/// </summary>
		/// <param name="receiveData">Results of the game battle</param>
		public void ReceiveGameData(Dictionary<GameResultType, int> receiveData) {
			menuModule.SetupScoreScreen(receiveData);
			menuModule.UpdateHighscores(bank.PlayerName, receiveData[GameResultType.Points]);
			menuModule.ChangeMenu("scorescreen");
			Camera.MoveCameraTo(0, 0);

			stateMachine.Fire(Trigger.TOGGLE);
		}
	}
}
