using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using System.IO;
using Newtonsoft.Json;
using Stateless;

namespace Wormhole
{
	public class GameController : IWindow
	{
		//window
		public Size2D<int> Size { get; private set; }
		public string Title { get; private set; }
		public Color Clr { get; private set; }

		//resource path
		private readonly string resourcePath;

		//States
		private enum State { MENU, GAME };
		private enum Trigger { TOGGLE };
		private StateMachine<State, Trigger> stateMachine;

		//player profile
		private Player player;

		//factories
		private LevelFactory levelFac;
		private WHGameFactory gameFac;
		private MenuFactory menuFac;
		private ShipFactory shipFac;

		//modules
		private IMenuModule menuModule;
		private IGameModule gameModule;
		//what to pass to the game
		//selected ship
		//selected level(difficulty)
		/////////////////////////////
		//what to get from the game
		//money
		//ship status
		//level status/score

		public GameController(string t, int w, int h, Color c)
		{
			Size = new Size2D<int>(w, h);
			Title = t;
			Clr = c;

			resourcePath = SwinGame.AppPath() + "\\Resources"; //TODO CHANGE THIS TO RESOURCE FOLDER

			SwinGame.OpenGraphicsWindow(Title, Size.W, Size.H);
		}
		public GameController() : this("NoName", 400, 400, Color.Black) { }

		//resize the window
		public void SetWindow(int w, int h)
		{
			SwinGame.ChangeWindowSize(Title, w, h);
			Size = new Size2D<int>(w, h);
		}

		private void LoadResources()
		{
			SwinGame.OpenAudio();
			//load music and other stuff
		}

		private void Init()
		{
			LoadResources();

			//Player profile
			player = new Player(resourcePath + "\\progress\\progress.json");

			//Factories
			levelFac = new LevelFactory(resourcePath + "\\levels");
			gameFac = new WHGameFactory(resourcePath + "\\WHGame");
			menuFac = new MenuFactory(resourcePath + "\\menus");
			shipFac = new ShipFactory(resourcePath + "\\entities\\ships");

			menuModule = menuFac.Create(player, shipFac.BuildShipList(), levelFac.BuildLevelList());			
			gameModule = gameFac.Create(player, shipFac.Fetch("testShip"), levelFac.Fetch("Level1"));

			//State
			stateMachine = new StateMachine<State, Trigger>(State.MENU);
			stateMachine.Configure(State.MENU)
				.Permit(Trigger.TOGGLE, State.GAME)
				.OnExit(() => FetchModuleProgress(menuModule)) //get progress information from module
				.OnEntry(() => CreateMenuModule()); //send updated progress information to module
			stateMachine.Configure(State.GAME)
				.Permit(Trigger.TOGGLE, State.MENU)
				.OnExit(() => FetchModuleProgress(gameModule)) //get progress information from module
				.OnEntry(() => CreateGameModule()); //send progress information to module
		}

		public void Run()
		{
			Init();

			while (!SwinGame.WindowCloseRequested())
			{
				SwinGame.ClearScreen(Clr);
				SwinGame.ProcessEvents();

				Update();
				Draw();

				SwinGame.DrawFramerate(0, 0);
				SwinGame.RefreshScreen(60);
			}
		}

		public void UpdateProgress(Player p)
		{
			player = p;
		}

		public void Update()
		{
			//run updates and check for state change
			switch (stateMachine.State)
			{
				case (State.GAME):
					gameModule.Update();
					HandleModuleTransition(gameModule);
					break;
				case (State.MENU):
					menuModule.Update();
					HandleModuleTransition(menuModule);
					break;
			}
		}

		public void Draw()
		{
			switch (stateMachine.State)
			{
				case (State.GAME):
					//gameModule.Draw();
					break;
				case (State.MENU):
					menuModule.Draw();
					break;
			}

			SwinGame.DrawText(player.Balance().ToString(), Color.White, 50, 50);
		}

		//state on exit, on entry functions
		private void FetchModuleProgress(IModule m)
		{
			player = m.PlayerProgress;
			player.RemoveMoney(5);
			player.SaveProgress();
			Log.Msg("fetching module progress");
		}

		private void CreateMenuModule()
		{
			menuModule = menuFac.Create(player, shipFac.BuildShipList(), levelFac.BuildLevelList());
			Log.Msg("Creating menu module");
		}

		private void CreateGameModule()
		{
			gameModule = gameFac.Create(player, shipFac.Fetch("testShip"), levelFac.Fetch("Level1"));
			Log.Msg("Creating game module");
		}

		private void HandleModuleTransition(IModule module)
		{
			if (module.Ended || SwinGame.KeyTyped(KeyCode.SpaceKey))
			{
				Log.Msg("/////////////////");
				stateMachine.Fire(Trigger.TOGGLE);
				Console.WriteLine("module state triggered to " + stateMachine.State);
			}
		}
	}
}
