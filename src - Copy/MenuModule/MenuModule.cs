using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;

namespace TaskForceUltra.src.MenuModule
{
	public delegate void ExitGame();

	public class MenuModule : IMenuModule
	{
		private List<Menu> menus;
		private Menu currMenu;
		private Level menuScene;
		private MenuSendData selections;
		private EntityHandler entityHandler;

		private Dictionary<string, Shape> shipList;
		private Dictionary<string, Level> levelList;
		private Bank bank;

		private ExitGame exitDelegate;

		public MenuModule(MenuSendData selections, Bank bank,
			Dictionary<string, Shape> shipList, Dictionary<string, Level> levelList,
			Level menuScene, EntityHandler entityHandler, ExitGame exit
		)
		{
			this.bank = bank;
			this.shipList = shipList;
			this.levelList = levelList;
			this.menuScene = menuScene;
			this.selections = selections;
			this.entityHandler = entityHandler;
			exitDelegate = exit;

			menus = new List<Menu>();
		}

		public void Update() {
			menuScene.Update();
			entityHandler.Update();
			currMenu.Update();
		}

		public void Draw() {
			menuScene.Draw();
			entityHandler.Draw(menuScene.PlayArea);
			currMenu.Draw();
		}

		/// <summary>
		/// Add a menu object to the menu module
		/// </summary>
		/// <param name="menu">Menu object</param>
		public void AddMenu(Menu menu) {
			if (MenuExists(menu.Id))
				RemoveMenu(menu.Id);

			menus.Add(menu);
		}

		private bool MenuExists(string id) {
			return menus.Exists(x => x.Id == id);
		}

		private Menu FetchMenu(string id) {
			return menus.Find(x => x.Id == id);
		}

		private void RemoveMenu(string id) {
			menus.Remove(FetchMenu(id));
		}

		/// <summary>
		/// Change the currently displayed menu
		/// </summary>
		/// <param name="id">id of the menu you want to change to</param>
		public void ChangeMenu(string id) {
			Menu foundMenu = menus.Find(x => (x.Id == id.ToLower()));
			if (foundMenu != null) {
				currMenu = foundMenu;
				currMenu?.ResetButtons();
			}
			else Console.WriteLine($"Can't find menu of id: {id}");
		}

		/// <summary>
		/// Adds player selected game option to the data that will be sent to the game controller
		/// </summary>
		/// <param name="selection">game option</param>
		/// <param name="id">id of the selected option</param>
		public void AddSelection(SelectionType selection, string id) {
			selections.Add(selection, id);
		}

		/// <summary>
		/// Removes player selected game option from the data sent to the game controller
		/// </summary>
		/// <param name="selection">game option</param>
		public void RemoveSelection(SelectionType selection) {
			selections.Remove(selection);
		}

		public void Send() {
			selections.Send();
		}

		public void Exit() {
			exitDelegate();
		}

		/// <summary>
		/// Setup the score sceen
		/// </summary>
		/// <param name="receiveData">Data object sent by the Game module</param>
		public void SetupScoreScreen(Dictionary<GameResultType, int> receiveData) {
			int points = receiveData[GameResultType.Points];
			bank.AddCredits(points);

			string battleResult;
			if ((int)receiveData[GameResultType.Result] == (int)BattleResult.Win)
				battleResult = "Victory";
			else battleResult = "Defeat";

			int totalTime = receiveData[GameResultType.Time];

			Menu menu = FetchMenu("scorescreen");
			menu?.InsertText("pointsText", points.ToString());
			menu?.InsertText("battleResultText", battleResult);
			menu?.InsertText("totalTimeText", (totalTime/1000).ToString());
		}

		/// <summary>
		/// update the high score menu with the game results
		/// </summary>
		/// <param name="name">name of player</param>
		/// <param name="points">player score</param>
		public void UpdateHighscores(string name, int points) {
			HighscoreMenu menu = FetchMenu("highscores") as HighscoreMenu;

			if (menu == null) {
				Console.WriteLine("cannot find highscore menu");
				return;
			}

			if (menu.TryInsertScore(name, points))
				return;
			else FetchMenu("scorescreen")?.RemoveElement("newHighscoreText");
		}
	}

	/// <summary>
	/// Menu Module Factory
	/// </summary>
	public class MenuModuleFactory
	{
		private string dirPath;

		public MenuModuleFactory(string dirPath) {
			this.dirPath = dirPath;
		}

		public MenuModule Create(Bank bank, Dictionary<string, Shape> shipList,
			Dictionary<string, Level> levelList, IReceiveMenuData receiver, ExitGame exit) {

			//spawn asteroids
			Level menuScene = levelList["MenuScene"];
			EntityHandler entityHandler = new EntityHandler(null, menuScene.PlayArea);
			AsteroidFactory asteroidFac = new AsteroidFactory();
			string asteroidPath = SwinGame.AppPath() + "\\resources\\data\\asteroids\\asteroid.json";
			for (int i = 0; i < menuScene.AsteroidsToSpawn; i++) {
				Asteroid toSpawn = asteroidFac.Create(asteroidPath, menuScene.PlayArea);
				entityHandler.Track(toSpawn);
			}

			MenuModule result = new MenuModule(new MenuSendData(receiver), bank, shipList, levelList, menuScene, entityHandler, exit);

			MenuFactory menuFac = new MenuFactory(dirPath, result);

			result.AddMenu(menuFac.CreateNormalMenu("\\help.json"));
			result.AddMenu(menuFac.CreateHighscoreMenu("\\highscores.json"));
			result.AddMenu(menuFac.CreateNormalMenu("\\main.json"));
			result.AddMenu(menuFac.CreateNormalMenu("\\options.json"));
			result.AddMenu(menuFac.CreateNormalMenu("\\scorescreen.json"));
			result.AddMenu(menuFac.CreateSelectMenu("\\select.json", shipList, levelList));

			result.ChangeMenu("Main");

			return result;
		}
	}
}
