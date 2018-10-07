using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.MenuModule
{
	public delegate void ExitGame();

	public class MenuModule : IMenuModule
	{
		private List<Menu> menus;
		private Menu currMenu;
		private Level menuScene;
		private MenuSendData selections;

		private Dictionary<string, Shape> shipList;
		private Dictionary<string, Level> levelList;
		private Bank bank;

		private ExitGame exitDelegate;

		public MenuModule(MenuSendData selection, Bank bank,
			Dictionary<string, Shape> shipList, Dictionary<string, Level> levelList,
			Level menuScene, ExitGame exit
		)
		{
			this.bank = bank;
			this.shipList = shipList;
			this.levelList = levelList;
			this.menuScene = menuScene;
			this.selections = selection;
			exitDelegate = exit;

			menus = new List<Menu>();
		}

		public void Update() {
			menuScene.Update();
			currMenu.Update();
		}

		public void Draw() {
			menuScene.Draw();
			currMenu.Draw();
		}

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

		public void ChangeMenu(string id) {
			Menu foundMenu = menus.Find(x => (x.Id == id.ToLower()));
			if (foundMenu != null) {
				currMenu = foundMenu;
				currMenu?.ResetButtons();
			}
			else Console.WriteLine($"Can't find menu of id: {id}");
		}

		public void AddSelection(SelectionType selection, string id) {
			this.selections.Add(selection, id);
		}

		public void RemoveSelection(SelectionType selection) {
			this.selections.Remove(selection);
		}

		public void Send() {
			selections.Send();
		}

		public void Exit() {
			exitDelegate();
		}

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

		public void UpdateHighscores(string name, int score) {
			HighscoreMenu menu = FetchMenu("highscores") as HighscoreMenu;

			if (menu == null) {
				Console.WriteLine("cannot find highscore menu");
				return;
			}

			if (menu.IsHighscore(score)) {
				menu.InsertScore(name, score);
			}
			else menu.RemoveElement("newHighscoreText");
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
			//create module, then add menus to it
			MenuModule result = new MenuModule(new MenuSendData(receiver), bank, shipList, levelList, levelList["MenuScene"], exit);

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
