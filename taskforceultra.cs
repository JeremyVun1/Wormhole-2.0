using System;
using SwinGameSDK;
using TaskForceUltra.src;

namespace TaskForceUltra
{
	public class GameMain
	{
		public static void Main()
		{
			GameController TaskForceUltra = new GameController();
			TaskForceUltra.Run();
		}
	}
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra
{
	public interface IModuleInterface : IReceiveMenuData, IReceiveGameData
	{
	}

	public interface IReceiveGameData
	{
		void ReceiveGameData(Dictionary<GameResultType, int> receiveData);
	}

	public interface IReceiveMenuData
	{
		void ReceiveMenuData(Dictionary<SelectionType, string> receiveData);
	}
}

﻿using System;
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using System.IO;

namespace TaskForceUltra
{
	/// <summary>
	/// Player moneys and id
	/// </summary>
	public class Bank
	{
		[JsonProperty("id")]
		public string PlayerName { get; private set; }
		[JsonProperty("credits")]
		public int Credits { get; private set; }
		[JsonIgnore]
		private string filePath;

		public Bank(string filePath, int credits = 0) {
			Credits = credits;
			this.filePath = filePath;
			Load();
		}

		public void AddCredits(int credits) {
			Credits += credits;
		}

		public void RemoveCredits(int credits) {
			AddCredits(-credits);
			Credits = credits.Clamp(0, Credits);
		}

		/// <summary>
		/// Load from file
		/// </summary>
		public void Load() {
			try {
				string jsonStr = File.ReadAllText(filePath);

				JsonConvert.PopulateObject(jsonStr, this);
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}

		/// <summary>
		/// Save to file
		/// </summary>
		public void Save() {
			try {
				string jsonStr = JsonConvert.SerializeObject(this);
				Console.WriteLine(jsonStr);
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}
	}
}

﻿using System;
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
			EntityHandler entityHandler = new EntityHandler(null);
			Level menuScene = levelList["MenuScene"];
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

﻿namespace TaskForceUltra
{
	public interface IMenuModule
	{
		void AddSelection(SelectionType selection, string id);
		void ChangeMenu(string id);
		void Exit();
		void RemoveSelection(SelectionType selection);
		void Send();
	}
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Selected Game options for sending to the game controller to start a game
	/// </summary>
	public class MenuSendData
	{
		private IReceiveMenuData receiver;
		private Dictionary<SelectionType, string> selections;

		public MenuSendData(IReceiveMenuData receiver) {
			selections = new Dictionary<SelectionType, string>();
			this.receiver = receiver;
		}

		public void Send() {
			//check that all possible selection types are selected
			if (selections.Count() == Enum.GetNames(typeof(SelectionType)).Length)
				receiver.ReceiveMenuData(selections);
		}

		public void Add(SelectionType selection, string id) {
			if (selections.ContainsKey(selection))
				Remove(selection);
			selections.Add(selection, id);
		}

		public void Remove(SelectionType selection) {
			selections.Remove(selection);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// A menu object that handles buttons and textboxes
	/// </summary>
	public class Menu
	{
		public string Id { get; private set; }
		private string title;
		protected List<MenuElement> elements;
		private Rectangle bounds;

		public Menu(string id, string title, List<MenuElement> elements) {
			Id = id;
			this.title = title;
			this.elements = elements;
			bounds = SwinGame.CreateRectangle(0, 0, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}

		public virtual void Update() {
			foreach(Button b in elements.OfType<Button>()) {
				b.Update();
			}
		}

		public virtual void Draw() {
			foreach(MenuElement e in elements) {
				e.Draw();
			}

			SwinGame.DrawText(title, Color.White, Color.Transparent, "MenuTitle", FontAlignment.AlignCenter, bounds);
		}

		/// <summary>
		/// Reset the state of children buttons
		/// </summary>
		public void ResetButtons() {
			foreach(Button b in elements.OfType<Button>()) {
				b.Reset();
			}
		}

		/// <summary>
		/// Insert text into a textbox on the menu
		/// </summary>
		/// <param name="elementId">textbox id</param>
		/// <param name="text">textbox text</param>
		public void InsertText(string elementId, string text) {
			MenuElement e = FetchElement(elementId);
			if (e != null)
				e.text = text;
		}

		/// <summary>
		/// Remove an element from the menu
		/// </summary>
		public void RemoveElement(string elementId) {
			MenuElement e = FetchElement(elementId);
			if (e != null)
				elements.Remove(e);
		}

		/// <summary>
		/// fetch an element from the meny by id
		/// </summary>
		/// <returns>menu element or null</returns>
		private MenuElement FetchElement(string id) {
			if (elements != null) {
				foreach (MenuElement e in elements) {
					if (e.id == id) {
						return e;
					}
				}
			}

			return null;
		}
	}

	/// <summary>
	/// Menu Factory
	/// </summary>
	public class MenuFactory
	{
		private string dirPath;
		private IMenuModule menuModule;
		private MenuElementFactory menuElementFac;

		public MenuFactory(string dirPath, IMenuModule menuModule) {
			this.dirPath = dirPath;
			this.menuModule = menuModule;
			menuElementFac = new MenuElementFactory();
		}

		public Menu CreateNormalMenu(string fileName) {
			JObject menuObj = Util.Deserialize(dirPath + fileName);
			string id = menuObj.Value<string>("id").ToLower();
			string title = menuObj.Value<string>("title");

			JArray textBoxesObj = menuObj.Value<JArray>("textBoxes");
			JArray buttonsObj = menuObj.Value<JArray>("buttons");
			JArray colorsObj = menuObj.Value<JArray>("elementColors");

			List<MenuElement> elements = menuElementFac.Create(textBoxesObj, buttonsObj, colorsObj, menuModule);

			return new Menu(id, title, elements);
		}

		public HighscoreMenu CreateHighscoreMenu(string fileName) {
			JObject menuObj = Util.Deserialize(dirPath + fileName);
			string id = menuObj.Value<string>("id").ToLower();
			string title = menuObj.Value<string>("title");

			JArray textBoxesObj = menuObj.Value<JArray>("textBoxes");
			string pathname = textBoxesObj[0].Value<string>("path");
			Console.WriteLine(pathname);

			JArray buttonsObj = menuObj.Value<JArray>("buttons");
			JArray colorsObj = menuObj.Value<JArray>("elementColors");

			string buffer = File.ReadAllText(SwinGame.AppPath() + pathname);
			textBoxesObj = JsonConvert.DeserializeObject<JArray>(buffer);

			List<MenuElement> elements = menuElementFac.Create(textBoxesObj, buttonsObj, colorsObj, menuModule);

			return new HighscoreMenu(id, title, elements);
		}

		public Menu CreateSelectMenu(string fileName, Dictionary<string, Shape> shipList, Dictionary<string, Level> levelList) {
			JObject menuObj = Util.Deserialize(dirPath + fileName);
			string id = menuObj.Value<string>("id").ToLower();
			string title = menuObj.Value<string>("title");
			JArray textBoxesObj = menuObj.Value<JArray>("textBoxes");
			JArray buttonsObj = menuObj.Value<JArray>("buttons");
			JArray colorsObj = menuObj.Value<JArray>("elementColors");
			List<MenuElement> elements = menuElementFac.Create(textBoxesObj, buttonsObj, colorsObj, menuModule);

			int sw = SwinGame.ScreenHeight();
			int sh = SwinGame.ScreenWidth();

			List<Rectangle> shipSelectionBounds = CreateSelectionBounds(0.1f*sw, 0.2f*sh, 0.05f*sw, 0.01f*sh, 0.2f*sw, 0.04f*sh);
			List<Rectangle> difficultySelectionBounds = CreateSelectionBounds(0.1f*sw, 0.32f*sh, 0.05f*sw, 0.01f*sh, 0.2f*sw, 0.04f*sh);
			List<Rectangle> levelSelectionBounds = CreateSelectionBounds(0.1f*sw, 0.44f*sh, 0.05f*sw, 0.01f*sh, 0.2f*sw, 0.04f*sh);

			SelectionGroup shipSelection = new SelectionGroup(SelectionType.Ship);
			SelectionGroup difficultySelection = new SelectionGroup(SelectionType.Difficulty);
			SelectionGroup levelSelection = new SelectionGroup(SelectionType.Level);

			//create ship selection buttons
			int n = shipList.Count > shipSelectionBounds.Count ? shipSelectionBounds.Count : shipList.Count;
			for (int i = 0; i < n; ++i) {
				string shipId = shipList.ElementAt(i).Key;
				shipSelection.Add(menuElementFac.CreateSelectButton(id, shipId, shipSelectionBounds[i], SelectionType.Ship, shipId, shipSelection, menuModule));
			}

			//create level selection buttons
			n = levelList.Count > levelSelectionBounds.Count ? levelSelectionBounds.Count : levelList.Count;
			for (int i = 0; i < n; ++i) {
				string levelId = levelList.ElementAt(i).Key;
				if (levelList.ElementAt(i).Value.Playable) // only allow it to be selected if the level is playable
					levelSelection.Add(menuElementFac.CreateSelectButton(id, levelId, levelSelectionBounds[i], SelectionType.Level, levelId, levelSelection, menuModule));
			}

			//difficulty buttons
			difficultySelection.Add(menuElementFac.CreateSelectButton("easy", "Easy", difficultySelectionBounds[0], SelectionType.Difficulty, "easy", difficultySelection, menuModule));
			difficultySelection.Add(menuElementFac.CreateSelectButton("medium", "Medium", difficultySelectionBounds[1], SelectionType.Difficulty, "medium", difficultySelection, menuModule));
			difficultySelection.Add(menuElementFac.CreateSelectButton("hard", "Hard", difficultySelectionBounds[2], SelectionType.Difficulty, "hard", difficultySelection, menuModule));

			MenuCommandFactory menuCommandFac = new MenuCommandFactory(menuModule);

			return new SelectionMenu(id, title, elements, shipSelection, difficultySelection, levelSelection);
		}

		private List<Rectangle> CreateSelectionBounds(float xStart, float yStart, float xPadding, float yPadding, float width, float height) {
			List<Rectangle> result = new List<Rectangle>();
			for (int y = 0; y < 2; ++y) {
				for (int x = 0; x < 4; ++x) {
					float rectX = xStart + (x * (width + xPadding));
					float rectY = yStart + (y * (height + yPadding));
					Rectangle bounds = SwinGame.CreateRectangle(rectX, rectY, width, height);
					result.Add(bounds);
				}
			}
			return result;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra.src.MenuModule
{
	public class HighscoreMenu : Menu
	{
		private string scoresFile;
		private List<KeyValuePair<string, int>> highscores;

		public HighscoreMenu(string id, string title, List<MenuElement> elements) : base(id, title, elements) {
			highscores = new List<KeyValuePair<string, int>>();
			scoresFile = SwinGame.AppPath() + "\\resources\\data\\scores.json";

			InitHighscores();
		}

		public override void Update() {
			base.Update();
		}

		/// <summary>
		/// Initialise the highscores keyvaluepair list to keep track of highscores in memory
		/// </summary>
		private void InitHighscores() {
			string key = "";
			int count = 0, i = 0, value = 0;

			//iterate through textboxes and get values
			foreach (TextBox t in elements.OfType<TextBox>()) {
				if (t.id == $"name{i}") {
					key = t.text;
					count += 1;
				}
				else if (t.id == $"score{i}") {
					Int32.TryParse(t.text, out value);
					count += 1;
				}

				if (count == 2) {
					KeyValuePair<string, int> entry = new KeyValuePair<string, int>(key, value);
					highscores.Add(entry);
					count = 0;
					i += 1;
				}
			}
		}

		/// <summary>
		/// update the textboxes based on highscore data
		/// </summary>
		private void UpdateTextBoxes() {
			for (int i = 0; i < highscores.Count; ++i) {
				InsertText($"name{i}", highscores.ElementAt(i).Key);
				InsertText($"score{i}", highscores.ElementAt(i).Value.ToString());
			}
		}

		/// <summary>
		/// checks whether the score is a high score
		/// </summary>
		/// <param name="points">player score</param>
		/// <returns>true or false</returns>
		private bool IsHighscore(int points) {
			if (highscores.Count < 10)
				return true;

			for(int i=0; i<highscores.Count; ++i) {
				if (points > highscores.ElementAt(i).Value)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Attempts to insert score into the high scores
		/// </summary>
		/// <param name="name">name of the player</param>
		/// <param name="points">score they got</param>
		/// <returns>returns true if got a highscore, false if not highscore</returns>
		public bool TryInsertScore(string name, int points) {
			if (!IsHighscore(points)) {
				return false;
			}

			highscores.Add(new KeyValuePair<string, int>(name, points));

			SortTrimHighscores();
			UpdateTextBoxes();

			Save();
			return true;
		}

		/// <summary>
		/// Order highscores descending and remove lowest score
		/// </summary>
		private void SortTrimHighscores() {
			var sorted = highscores.OrderByDescending(x => x.Value);
			int numScores = sorted.Count() < 10 ? sorted.Count() : 10;

			List<KeyValuePair<string, int>> newHighscores = new List<KeyValuePair<string, int>>();
			for (int i = 0; i < numScores; i++) {
				var result = new KeyValuePair<string, int>(sorted.ElementAt(i).Key, sorted.ElementAt(i).Value);
				newHighscores.Add(result);
			}
			highscores = newHighscores;
		}

		/// <summary>
		/// save dictionary to file
		/// </summary>
		private void Save() {
			var textBoxes = elements.OfType<TextBox>();

			string jsonString = JsonConvert.SerializeObject(textBoxes, Formatting.Indented);
			File.WriteAllText(scoresFile, jsonString);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Menu which allows player to select game options
	/// </summary>
	public class SelectionMenu : Menu
	{
		private SelectionGroup selectShipButtons;
		private SelectionGroup selectDifficultyButtons;
		private SelectionGroup selectLevelButtons;

		public SelectionMenu(string id, string title, List<MenuElement> elements,
			SelectionGroup ships, SelectionGroup diff, SelectionGroup levels
		) : base(id, title, elements) 
		{
			selectShipButtons = ships;
			selectDifficultyButtons = diff;
			selectLevelButtons = levels;
		}

		public override void Update() {
			base.Update();
			selectShipButtons.Update();
			selectDifficultyButtons.Update();
			selectLevelButtons.Update();
		}

		public override void Draw() {
			base.Draw();
			selectShipButtons.Draw();
			selectDifficultyButtons.Draw();
			selectLevelButtons.Draw();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// base class for buttons and textboxes etc.
	/// </summary>
	public abstract class MenuElement {
		protected Color hoverColor { get; private set; }
		private Color fillColor;
		private Color borderColor;
		protected Color fontColor { get; private set; }

		[JsonProperty("text")]
		public string text;
		protected string fontId { get; private set; }
		protected FontAlignment alignment { get; private set; }
		protected Rectangle bounds { get; private set; }

		[JsonProperty("pos")]
		private Point2D pos {
			get {
				return SwinGame.PointAt(bounds.X / SwinGame.ScreenWidth(), bounds.Y / SwinGame.ScreenHeight());
			}
		}
		[JsonProperty("size")]
		private Size2D<float> size {
			get {
				return new Size2D<float>(bounds.Width / SwinGame.ScreenWidth(), bounds.Height / SwinGame.ScreenHeight());
			}
		}

		[JsonProperty("id")]
		public string id { get; private set; }

		public MenuElement(string id, Rectangle bounds, Color hover, Color fill, Color border,
			Color font, string text, string fontId, FontAlignment alignment
		)
		{
			this.id = id;
			hoverColor = hover;
			fillColor = fill;
			borderColor = border;
			fontColor = font;
			this.text = text;
			this.bounds = bounds;
			this.fontId = fontId;
			this.alignment = alignment;
		}

		public virtual void Draw() {
			SwinGame.FillRectangle(fillColor, bounds);
			DrawOutline();
			SwinGame.DrawText(text, fontColor, Color.Transparent, fontId, FontAlignment.AlignCenter, bounds);
		}

		protected virtual void DrawOutline() {
				SwinGame.DrawRectangle(borderColor, bounds);
		}
	}

	/// <summary>
	/// Menu Element Factory
	/// </summary>
	public class MenuElementFactory
	{
		public List<MenuElement> Create(JArray textBoxesObj, JArray buttonsObj, JArray colorsObj, IMenuModule menuModule) {
			List<MenuElement> result = new List<MenuElement>();

			Color hoverColor = Util.DeserializeKeyedColor(colorsObj, "hoverColor");
			Color fillColor = Util.DeserializeKeyedColor(colorsObj, "fillColor");
			Color borderColor = Util.DeserializeKeyedColor(colorsObj, "borderColor");
			Color fontColor = Util.DeserializeKeyedColor(colorsObj, "fontColor");

			foreach (JObject obj in textBoxesObj) {
				result.Add(CreateTextBox(obj, hoverColor, fillColor, borderColor, fontColor));
			}

			foreach (JObject obj in buttonsObj) {
				result.Add(CreateButton(obj, hoverColor, fillColor, borderColor, fontColor, menuModule));
			}

			return result;
		}

		public TextBox CreateTextBox(JObject textObj, Color hover, Color fill, Color border, Color font) {
			Rectangle bounds = CreateElementBounds(textObj);
			string text = textObj.Value<string>("text");
			string id = textObj.Value<string>("id");

			return new TextBox(id, bounds, hover, fill, border, font, text);
		}

		public Button CreateButton(JObject buttonObj, Color hover, Color fill, Color border, Color font, IMenuModule menuModule) {
			Rectangle bounds = CreateElementBounds(buttonObj);
			string text = buttonObj.Value<string>("label");
			string action = buttonObj.Value<string>("action");
			string payload = buttonObj.Value<string>("payload");
			string id = buttonObj.Value<string>("id");
			string type = buttonObj.Value<string>("type");

			//build command for button
			MenuCommandFactory menuCommandFac = new MenuCommandFactory(menuModule);
			ICommand command = menuCommandFac.Create(action, payload);

			switch(type.ToLower()) {
				case "nonstick":
					return new NonStickButton(id, command, bounds, hover, fill, border, font, text);
				default:
					return new Button(id, command, bounds, hover, fill, border, font, text);
			}
		}

		/// <summary>
		/// built on runtime to allow the plaeyr to choose different data selections
		/// </summary>
		/// <returns></returns>
		public SelectButton CreateSelectButton(string id, string label, Rectangle bounds, SelectionType type, string payload, SelectionGroup parent, IMenuModule menuModule)
		{
			MenuCommandFactory menuCommandFac = new MenuCommandFactory(menuModule);
			ICommand command;

			switch (type) {
				case SelectionType.Difficulty:
					command = new SelectDifficultyCommand(menuModule, payload);
					break;
				case SelectionType.Level:
					command = new SelectLevelCommand(menuModule, payload);
					break;
				case SelectionType.Ship:
					command = new SelectShipCommand(menuModule, payload);
					break;
				default:
					command = null;
					break;
			}
			Color hover = Color.Orange;
			Color fill = Color.Grey;
			Color border = Color.White;
			Color font = Color.White;

			return new SelectButton(id, command, bounds, hover, fill, border, font, payload, parent, type);
		}

		private Rectangle CreateElementBounds(JObject obj) {
			Point2D pos = obj["pos"].ToObject<Point2D>();
			Size2D<float> size = obj["size"].ToObject<Size2D<float>>();
			int w = SwinGame.ScreenWidth();
			int h = SwinGame.ScreenHeight();

			return SwinGame.CreateRectangle(pos.X * w, pos.Y * h, size.W * w, size.H * h);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// generic button - has sticky clicking behaviour
	/// </summary>
	public class Button : MenuElement
	{
		private ICommand command;

		private enum State { HOVERED, CLICKED, REST }
		private enum Trigger { CLICK, HOVER, UNHOVER, RESET };
		private StateMachine<State, Trigger> stateMachine;

		public bool IsSelected {
			get {
				return stateMachine.State == State.CLICKED;
			}
		}

		public Button(string id, ICommand command, Rectangle bounds, Color hover, Color fill,
			Color border, Color font, string text
		) : base(id, bounds, hover, fill, border, font, text, "ButtonText", FontAlignment.AlignCenter)
		{
			this.command = command;
			stateMachine = new StateMachine<State, Trigger>(State.REST);
			ConfigureStateMachine();
		}

		public void Update() {
			if (ButtonHovered())
				stateMachine.Fire(Trigger.HOVER);
			else stateMachine.Fire(Trigger.UNHOVER);

			if (ButtonClicked())
				Click();
		}

		private void ConfigureStateMachine() {
			stateMachine.Configure(State.CLICKED)
				.OnEntry(() => Execute())
				.Permit(Trigger.CLICK, State.REST)
				.Permit(Trigger.RESET, State.REST)
				.Ignore(Trigger.HOVER)
				.Ignore(Trigger.UNHOVER);
			stateMachine.Configure(State.REST)
				.Permit(Trigger.CLICK, State.CLICKED)
				.Permit(Trigger.HOVER, State.HOVERED)
				.Ignore(Trigger.RESET)
				.Ignore(Trigger.UNHOVER);
			stateMachine.Configure(State.HOVERED)
				.Permit(Trigger.CLICK, State.CLICKED)
				.Permit(Trigger.UNHOVER, State.REST)
				.Permit(Trigger.RESET, State.REST)
				.Ignore(Trigger.HOVER);
		}

		private bool ButtonHovered() {
			return SwinGame.MousePosition().InRect(bounds);
		}

		private bool ButtonClicked() {
			return (ButtonHovered() && SwinGame.MouseClicked(MouseButton.LeftButton));
		}

		public override void Draw() {
			if (stateMachine.State != State.REST) {
				SwinGame.FillRectangle(hoverColor, bounds);
				SwinGame.DrawText(text, Color.Black, Color.Transparent, fontId, alignment, bounds);
				DrawOutline();
			}
			else base.Draw();
		}

		/// <summary>
		/// reset button state
		/// </summary>
		public void Reset() {
			stateMachine.Fire(Trigger.RESET);
		}

		/// <summary>
		/// click the button
		/// </summary>
		public virtual void Click() {
			stateMachine.Fire(Trigger.CLICK);
		}

		/// <summary>
		/// execute button command
		/// </summary>
		protected virtual void Execute() {
			command.Execute();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// a button without sticky clicking behaviour
	/// </summary>
	public class NonStickButton : Button
	{
		public NonStickButton(string id, ICommand command, Rectangle bounds, Color hover,
			Color fill, Color border, Color font, string text
		) : base(id, command, bounds, hover, fill, border, font, text) { }

		protected override void Execute() {
			base.Execute();
			Reset();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// a button that is part of a selection group. Player can only select one of these buttons within the group
	/// </summary>
	public class SelectButton : Button
	{
		public SelectionType groupType { get; private set; }
		private SelectionGroup parent;

		public SelectButton(string id, ICommand command, Rectangle bounds, Color hover,
			Color fill, Color border, Color font, string text, SelectionGroup parent, SelectionType selectionType
		) : base(id, command, bounds, hover, fill, border, font, text)
		{
			groupType = selectionType;
			this.parent = parent;
		}

		public override void Click() {
			parent.ResetButtons();
			base.Click();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Selection group for a set of select buttons. Can only pick one select button within the group
	/// </summary>
	public class SelectionGroup
	{
		public SelectionType selectionGroup { get; private set; }
		private List<Button> childButtons;

		public SelectionGroup(SelectionType type) {
			selectionGroup = type;
			childButtons = new List<Button>();
		}

		/// <summary>
		/// Add buttons to the selection group
		/// </summary>
		/// <param name="b">Button to add</param>
		public void Add(Button b) {
			if (b!=null)
				childButtons.Add(b);
		}
		
		/// <summary>
		/// Remove button from selection group
		/// </summary>
		/// <param name="b">Button to remove</param>
		public void Remove(Button b) {
			if (b != null && childButtons.Contains(b))
				childButtons.Remove(b);
		}

		public void Update() {
			foreach(Button b in childButtons) {
				b.Update();
			}
		}

		public void Draw() {
			foreach(Button b in childButtons) {
				b.Draw();
			}
		}

		public void ResetButtons() {
			foreach(Button b in childButtons) {
				b.Reset();
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// textbox for holding text
	/// </summary>
	public class TextBox : MenuElement
	{
		public TextBox(string id, Rectangle bounds, Color hover, Color fill,
			Color border, Color font, string text
		) : base(id, bounds, hover, Color.Transparent, border, font, text, "MenuText", FontAlignment.AlignLeft) { }

		protected override void DrawOutline() { }
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra
{
	/// <summary>
	/// Interface for implementing Command pattern
	/// </summary>
	public interface ICommand
	{
		void Execute();
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// factory for creating command objects for the menu module
	/// </summary>
	public class MenuCommandFactory
	{
		private IMenuModule menuModule;

		public MenuCommandFactory(IMenuModule m) {
			menuModule = m;
		}

		public ICommand Create(string action, string payload) {
			switch (action.ToLower()) {
				case "navto":
					return new NavToCommand(menuModule, payload);
				case "increasevolume":
					return new IncreaseVolumeCommand();
				case "decreasevolume":
					return new DecreaseVolumeCommand();
				case "exit":
					return new ExitMenuCommand(menuModule);
				case "selectship":
					return new SelectShipCommand(menuModule, payload);
				case "selectlevel":
					return new SelectLevelCommand(menuModule, payload);
				case "selectdifficulty":
					return new SelectDifficultyCommand(menuModule, payload);
				case "play":
					return new PlayCommand(menuModule);
				default:
					return null;
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to navigate to another menu
	/// </summary>
	public class NavToCommand : ICommand
	{
		private IMenuModule menuModule;
		private string id;

		public NavToCommand(IMenuModule m, string id) {
			menuModule = m;
			this.id = id;
		}

		public void Execute() {
			menuModule.ChangeMenu(id);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to start playing the game
	/// </summary>
	public class PlayCommand : ICommand
	{
		private IMenuModule menuModule;

		public PlayCommand(IMenuModule m) {
			menuModule = m;
		}

		public void Execute() {
			menuModule.Send();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to select a difficulty to play at
	/// </summary>
	public class SelectDifficultyCommand : ICommand
	{
		private IMenuModule menuModule;
		private string id;

		public SelectDifficultyCommand(IMenuModule m, string id) {
			menuModule = m;
			this.id = id;
		}

		public void Execute() {
			menuModule.AddSelection(SelectionType.Difficulty, id);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to select a level to play on
	/// </summary>
	public class SelectLevelCommand : ICommand
	{
		private IMenuModule menuModule;
		private string id;

		public SelectLevelCommand(IMenuModule m, string id) {
			menuModule = m;
			this.id = id;
		}

		public void Execute() {
			menuModule.AddSelection(SelectionType.Level, id);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to select a ship to play as
	/// </summary>
	public class SelectShipCommand : ICommand
	{
		private IMenuModule menuModule;
		private string id;

		public SelectShipCommand(IMenuModule m, string id) {
			menuModule = m;
			this.id = id;
		}

		public void Execute() {
			menuModule.AddSelection(SelectionType.Ship, id);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to increase swingame music volume
	/// </summary>
	public class IncreaseVolumeCommand : ICommand
	{
		public IncreaseVolumeCommand() { }

		public void Execute() {
			SwinGame.SetMusicVolume(SwinGame.MusicVolume() + 0.1f);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to decrease swingame music volume
	/// </summary>
	public class DecreaseVolumeCommand : ICommand
	{
		public DecreaseVolumeCommand() { }

		public void Execute() {
			SwinGame.SetMusicVolume(SwinGame.MusicVolume() - 0.1f);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to exit the game
	/// </summary>
	public class ExitMenuCommand : ICommand
	{
		private IMenuModule menuModule;

		public ExitMenuCommand(IMenuModule m)
		{
			menuModule = m;
		}

		public void Execute() {
			menuModule.Exit();
		}
	}
}

﻿using System;
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
			EntityHandler entHandler = new EntityHandler(scoreSheet);
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// bundles game result data to send to the game controller
	/// </summary>
	public class GameSendData
	{
		private IReceiveGameData receiver;
		private Dictionary<GameResultType, int> results;

		public GameSendData(IReceiveGameData receiver) {
			results = new Dictionary<GameResultType, int>();
			this.receiver = receiver;
		}

		public void Send() {
			if (results.Count() == Enum.GetNames(typeof(SelectionType)).Length)
				receiver.ReceiveGameData(results);
		}

		public void Add(GameResultType result, int x) {
			if (results.ContainsKey(result))
				Remove(result);
			results.Add(result, x);
		}

		public void Remove(GameResultType result) {
			results.Remove(result);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Manages the score of different teams
	/// </summary>
	public class Scoresheet
	{
		private Dictionary<Team, float> teamScores;

		public Scoresheet() {
			teamScores = new Dictionary<Team, float>();

			teamScores.Add(Team.Team1, 0);
			teamScores.Add(Team.Team2, 0);
			teamScores.Add(Team.Team3, 0);
			teamScores.Add(Team.Team4, 0);
			teamScores.Add(Team.Computer, 0);
		}

		/// <summary>
		/// Add points to a team
		/// </summary>
		public void AddPoints(Team team, float points) {
			teamScores[team] = teamScores[team] + points;
		}

		/// <summary>
		/// Remove points from a team
		/// </summary>
		public void RemovePoints(Team team, float points) {
			AddPoints(team, -points);
		}

		/// <summary>
		/// Fetch score of the team
		/// </summary>		
		/// <returns>score</returns>
		public int FetchTeamScore(Team team) {
			return (int)teamScores[team];
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwinGameSDK;
using TaskForceUltra.src.GameModule;

namespace TaskForceUltra
{
	/// <summary>
	/// Level scene with background stars and a defined play area
	/// </summary>
	public class Level
	{
		public string Id { get; private set; }
		private List<EnvMod> EnvMods; //TODO implement
		private Background background;
		public Rectangle PlayArea { get; private set; }
		public bool Playable { get; private set; }
		public List<string> ShipsToSpawn { get; private set; }
		public int AsteroidsToSpawn { get; private set; }

		public Level(string id, List<EnvMod> EnvMods, List<string> shipsToSpawn,
			int asteroidsToSpawn, bool playable, Rectangle playArea, Background bkgd)
		{
			Id = id;
			this.EnvMods = EnvMods;
			background = bkgd;
			PlayArea = playArea;
			ShipsToSpawn = shipsToSpawn;
			AsteroidsToSpawn = asteroidsToSpawn;
			Playable = playable;
		}

		public void Update() {
			background.Update();
		}

		public void Draw() {
			background.Draw();
		}
	}

	/// <summary>
	/// Level Factory
	/// </summary>
	public class LevelFactory
	{
		private string[] fileList;
		private string dirPath;
		public Dictionary<string,Level> levelList { get; private set; }

		public LevelFactory(string dirPath) {
			this.dirPath = dirPath;
			levelList = new Dictionary<string, Level>();
			RegisterLevels();
		}

		public void RegisterLevels() {
			levelList.Clear();

			fileList = Directory.GetFiles(dirPath);

			foreach (string file in fileList) {
				try {
					JObject obj = Util.Deserialize(file);
					if (obj == null)
						continue;

					string id = obj.Value<string>("id");
					bool playable = obj.Value<bool>("playable");
					string json = JsonConvert.SerializeObject(obj.GetValue("ships"));
					List<string> shipsToSpawn = JsonConvert.DeserializeObject<List<string>>(json);
					int asteroidsToSpawn = obj.Value<int>("asteroids");

					Size2D<int> size = obj["size"].ToObject<Size2D<int>>();
					Rectangle playArea = SwinGame.CreateRectangle(SwinGame.PointAt(0, 0), size.W, size.H);

					List<EnvMod> EnvMods = obj["environMods"].ToObject<List<EnvMod>>(); //Most likely will not work

					JObject bkgdObj = obj.Value<JObject>("background");
					BackgroundFactory backgroundFac = new BackgroundFactory();
					Background bkgd = backgroundFac.Create(bkgdObj, playArea);

					levelList.Add(id, new Level(id, EnvMods, shipsToSpawn, asteroidsToSpawn, playable, playArea, bkgd));
				}
				catch(Exception e) {
					Console.WriteLine($"Cannot read level: {file}");
					Console.WriteLine(e);
				}
			}
		}

		//search methods on level list
		public bool LevelExists(string levelId) {
			if (levelList.ContainsKey(levelId))
					return true;
			return false;
		}

		public Level Fetch(string levelId) {
			return levelList[levelId];
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwinGameSDK;

namespace TaskForceUltra
{
	/// <summary>
	/// Manage's the stars and background colour of a level
	/// </summary>
	public class Background
	{
		private List<Star> stars;
		private Color bkgdColor;

		public Background(List<Star> stars, Color bkgdColor)
		{
			this.stars = stars;
			this.bkgdColor = bkgdColor;
		}

		public void Update() {
			SwinGame.ClearScreen(bkgdColor);

			foreach(Star s in stars) {
				s.Update();
			}
		}

		public void Draw() {
			foreach(Star s in stars) {
				s.Draw();
			}
		}
	}

	/// <summary>
	/// Background Factory
	/// </summary>
	public class BackgroundFactory
	{
		public Background Create(JObject bkgdObj, Rectangle playArea) {
			//deserialise background properties
			//create stars
			int starCount = bkgdObj.Value<int>("starCount");
			JToken sizeObj = bkgdObj.GetValue("size");
			MinMax<int> sizeRange = new MinMax<int>(sizeObj.Value<int>("Min"), sizeObj.Value<int>("Max"));
			JToken dimObj = bkgdObj.GetValue("dimRate");
			MinMax<float> dimRange = new MinMax<float>(dimObj.Value<float>("Min"), dimObj.Value<float>("Max"));
			JToken flareObj = bkgdObj.GetValue("flareRate");
			MinMax<float> flareRange = new MinMax<float>(flareObj.Value<float>("Min"), flareObj.Value<float>("Max"));
			JArray starColorsObj = bkgdObj.Value<JArray>("colors");
			List<Color> starColors = Util.LoadColors(starColorsObj);

			StarFactory starFac = new StarFactory();
			List<Star> stars = starFac.CreateList(starCount, sizeRange, dimRange, flareRange, starColors, playArea);

			//create background
			JToken bkgdColorObj = bkgdObj.GetValue("backgroundColor");
			Color bkgdColor = Util.GetRGBColor(bkgdColorObj);

			return new Background(stars, bkgdColor);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaskForceUltra
{
	/// <summary>
	/// A star object that dims and flares randomly
	/// </summary>
	public class Star
	{
		private List<Color> colors;
		private int currColorIndex;
		private Point2D pos;
		private int size;
		private MinMax<int> sizeRange;

		//timing
		private Random rng;
		private MinMax<float> flareRate; //per second
		private MinMax<float> dimRate; //per second
		private float flareTrigger;
		private float dimTrigger;
		private CooldownHandler cdHandler;

		//state
		private enum State { REST, DIMMING }
		private enum Trigger { FLARE, RESET }
		private StateMachine<State, Trigger> stateMachine;

		public Star(List<Color> colors, MinMax<int> sizeMinMax, MinMax<float> dimRateMinMax,MinMax<float> flareRateMinMax, Rectangle playArea) {
			this.colors = colors;
			currColorIndex = 0;
			rng = new Random(Guid.NewGuid().GetHashCode());

			pos = Util.RandomPointInRect(playArea);
			sizeRange = sizeMinMax;
			size = sizeRange.Min;

			//timing and rates
			flareRate = flareRateMinMax;
			dimRate = dimRateMinMax;
			flareTrigger = 1 / (Util.RandomInRange(flareRate) / 1000);
			dimTrigger = 1 / (Util.RandomInRange(dimRate) / 1000);
			flareTrigger -= 1000;
			cdHandler = new CooldownHandler(flareTrigger);

			//state machine
			stateMachine = new StateMachine<State, Trigger>(State.REST);
			ConfigureStateMachine();
			stateMachine.Fire((Trigger)Util.Rand(2));

			if (stateMachine.State == State.REST)
				cdHandler = new CooldownHandler(flareTrigger);
			else cdHandler = new CooldownHandler(dimTrigger);

			cdHandler.StartCooldown();
		}

		private void ConfigureStateMachine() {
			stateMachine.Configure(State.REST)
				.OnEntry(() => Reset())
				.Permit(Trigger.FLARE, State.DIMMING)
				.Ignore(Trigger.RESET);
			stateMachine.Configure(State.DIMMING)
				.OnEntry(() => Flare())
				.Permit(Trigger.RESET, State.REST)
				.Ignore(Trigger.FLARE);
		}

		public void Draw() {
			SwinGame.FillRectangle(colors?[currColorIndex], pos.X, pos.Y, size, size);
		}

		public void Update() {
			switch (stateMachine.State) {
				case State.REST:
					if (!cdHandler.IsOnCooldown())
						stateMachine.Fire(Trigger.FLARE);
					break;
				case State.DIMMING:
					Dim();
					break;
			}
		}

		/// <summary>
		/// Flare the star
		/// </summary>
		private void Flare() {
			cdHandler.StartNewThreshhold(dimTrigger);
			size = rng.Next(sizeRange.Min, sizeRange.Max + 1);
			ChangeColor();
		}

		/// <summary>
		/// Dim the star
		/// </summary>
		private void Dim() {
			if (!cdHandler.IsOnCooldown()) {
				ChangeColor();
				DecrementSize();
			}
		}

		private void ChangeColor() {
			currColorIndex = SwinGame.Rnd(colors.Count);
		}

		private void DecrementSize() {
			if (size > sizeRange.Min)
				size--;
			else stateMachine.Fire(Trigger.RESET);
		}

		/// <summary>
		/// Reset state of the star
		/// </summary>
		private void Reset() {
			currColorIndex = 0;
			flareTrigger = 1 / (Util.RandomInRange(flareRate) / 1000);
			dimTrigger = 1 / (Util.RandomInRange(dimRate) / 1000);
			cdHandler.StartNewThreshhold(flareTrigger);
		}
	}

	/// <summary>
	/// Star Factory
	/// </summary>
	public class StarFactory
	{
		public Star Create(MinMax<int> sizeRange, MinMax<float> dimRange,
			MinMax<float> flareRange, List<Color> starColors, Rectangle playArea) {
			return new Star(starColors, sizeRange, dimRange, flareRange, playArea);
		}

		public List<Star> CreateList(int count, MinMax<int> sizeRange, MinMax<float> dimRange,
			MinMax<float> flareRange, List<Color> starColors, Rectangle playArea) {
			List<Star> result = new List<Star>();

			for (int i = 0; i < count; ++i) {
				result.Add(Create(sizeRange, dimRange, flareRange, starColors, playArea));
			}
			return result;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace TaskForceUltra
{
	/// <summary>
	/// Management of a swingame timer
	/// </summary>
	public class CooldownHandler
	{
		private Timer timer;
		private float threshhold;

		private enum State { COOLDOWN, READY };
		private enum Trigger { TOGGLE, RESET };
		private StateMachine<State, Trigger> stateMachine;

		public CooldownHandler(float ms) {
			threshhold = ms;

			stateMachine = new StateMachine<State, Trigger>(State.READY);
			timer = SwinGame.CreateTimer();

			ConfigureStateMachine();
		}

		private void ConfigureStateMachine() {
			stateMachine.Configure(State.READY)
				.OnEntry(() => timer.Stop())
				.Permit(Trigger.TOGGLE, State.COOLDOWN)
				.Ignore(Trigger.RESET);
			stateMachine.Configure(State.COOLDOWN)
				.OnEntry(() => timer.Start())
				.Permit(Trigger.TOGGLE, State.READY)
				.Permit(Trigger.RESET, State.READY);
		}

		private void Update() {
			switch (stateMachine.State) {
				case State.COOLDOWN:
					if (timer.Ticks > threshhold)
						stateMachine.Fire(Trigger.TOGGLE);
					break;
			}
		}

		/// <summary>
		/// Start the cooldown
		/// </summary>
		public void StartCooldown() {
			if (stateMachine.State == State.READY)
				stateMachine.Fire(Trigger.TOGGLE);
		}

		/// <summary>
		/// checks whether the timer is on cooldown or not
		/// </summary>
		/// <returns>true or false</returns>
		public bool IsOnCooldown() {
			Update();
			return (stateMachine.State == State.COOLDOWN);
		}

		/// <summary>
		/// Start the timer with a new cooldown threshhold
		/// </summary>
		/// <param name="ms">milliseconds</param>
		public void StartNewThreshhold(float ms) {
			threshhold = ms;
			stateMachine.Fire(Trigger.RESET);
			StartCooldown();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// handles game camera behaviour
	/// </summary>
	public class CameraHandler
	{
		private Rectangle playArea;
		private Size2D<int> windowSize;
		private Point2D camOffset;
		private Rectangle chaseArea;
		private List<Rectangle> cornerAreas;
		private List<Rectangle> sideAreas;

		public Rectangle Viewport {
			get { return SwinGame.CreateRectangle(Camera.CameraPos(), SwinGame.ScreenWidth(), SwinGame.ScreenHeight()); }
		}

		private Entity activeEntity;

		private int rectLoc;
		private State state;
		private enum State { CORNER, SIDE, CHASE };

		public CameraHandler(Entity ent, Rectangle playArea) {
			activeEntity = ent;
			this.playArea = playArea;
			windowSize = new Size2D<int>(SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
			camOffset = SwinGame.PointAt(windowSize.W / 2, windowSize.H / 2);

			chaseArea = BuildChaseArea();
			cornerAreas = BuildCornerAreas();
			sideAreas = BuildSideAreas();
		}

		public void Update() {
			if (DebugMode.IsDebugging(Debugging.Camera)) {
				DebugArea(cornerAreas, SwinGame.RGBAColor(255, 0, 0, 80));
				DebugArea(sideAreas, SwinGame.RGBAColor(255, 0, 0, 50));
				DebugArea(chaseArea, SwinGame.RGBAColor(255, 0, 0, 20));
			}

			UpdateState();

			switch (state) {
				case State.CORNER:
					CornerCam();
					break;
				case State.SIDE:
					SideCam();
					break;
				case State.CHASE:
					ChaseCam();
					break;
			}
		}

		private void UpdateState() {
			if (IsEntityIn(cornerAreas))
				state = State.CORNER;
			else if (IsEntityIn(sideAreas))
				state = State.SIDE;
			else state = State.CHASE;
		}

		/// <summary>
		/// checks whether the entity is within the specified rectangle bound
		/// </summary>
		/// <param name="rects">a rectangle bound</param>
		private bool IsEntityIn(List<Rectangle> rects) {
			for(int i=0; i<4; ++i) {
				if (SwinGame.PointInRect(activeEntity.RealPos, rects[i])) {
					rectLoc = i;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Camera behaviour when the player is in the corner of the play area
		/// lock camera x and y from moving
		/// </summary>
		private void CornerCam() {
			switch (rectLoc) {
				case 0:
					Camera.MoveCameraTo(CenterOn(chaseArea.TopLeft));
					break;
				case 1:
					Camera.MoveCameraTo(CenterOn(chaseArea.TopRight));
					break;
				case 2:
					Camera.MoveCameraTo(CenterOn(chaseArea.BottomRight));
					break;
				case 3:
					Camera.MoveCameraTo(CenterOn(chaseArea.BottomLeft));
					break;
			}
		}

		/// <summary>
		/// Camera behaviour when the player is on the side of the play area
		/// clamp either x or y depending on which side the player is on
		/// </summary>
		private void SideCam() {
			switch (rectLoc) {
				case 0: // top - clamp y
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(activeEntity.RealPos.X, chaseArea.Top)));
					break;
				case 1: //right - clamp x
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(chaseArea.Right, activeEntity.RealPos.Y)));
					break;
				case 2: //bottom - clamp y
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(activeEntity.RealPos.X, chaseArea.Bottom)));
					break;
				case 3: //left - clamp x
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(chaseArea.Left, activeEntity.RealPos.Y)));
					break;
			}
		}

		/// <summary>
		/// Free chasing camera behaviour when the player is within the middle of the play area
		/// </summary>
		private void ChaseCam() {
			Camera.MoveCameraTo(CenterOn(activeEntity.RealPos));
		}

		/// <summary>
		/// Offset camera position so that it is centered on the target point
		/// </summary>
		/// <param name="target">point to center camera on</param>
		/// <returns>Offset camera position</returns>
		private Point2D CenterOn(Point2D target) {
			return target.Subtract(camOffset);
		}

		/// <summary>
		/// Define the area in which the camera will freely chase the player
		/// inset of the play area by the camera offset
		/// </summary>
		/// <returns>Rectangle</returns>
		private Rectangle BuildChaseArea() {
			Point2D TopLeft = playArea.TopLeft.Add(camOffset);
			Point2D BottomRight = playArea.BottomRight.Subtract(camOffset);
			return SwinGame.CreateRectangle(TopLeft, BottomRight);
		}

		/// <summary>
		/// Define the corner areas of the play area
		/// </summary>
		/// <returns>List of rectangles</returns>
		private List<Rectangle> BuildCornerAreas() {
			//now that we know the chase cam rectangle
			return new List<Rectangle>() {
				SwinGame.CreateRectangle(playArea.TopLeft, chaseArea.TopLeft),
				SwinGame.CreateRectangle(chaseArea.Right, playArea.Top, playArea.Right, chaseArea.Top),
				SwinGame.CreateRectangle(chaseArea.BottomRight, playArea.BottomRight),
				SwinGame.CreateRectangle(playArea.Left, chaseArea.Bottom, chaseArea.Left, playArea.Bottom)
			};
		}

		/// <summary>
		/// Build the side areas of the play area
		/// </summary>
		/// <returns>List of rectangles</returns>
		private List<Rectangle> BuildSideAreas() {
			return new List<Rectangle>() {
				SwinGame.CreateRectangle(cornerAreas[0].TopRight, chaseArea.TopRight), //top
				SwinGame.CreateRectangle(cornerAreas[1].BottomLeft, cornerAreas[2].TopRight), //right
				SwinGame.CreateRectangle(chaseArea.BottomLeft, cornerAreas[2].BottomLeft), //bottom
				SwinGame.CreateRectangle(cornerAreas[0].BottomLeft, chaseArea.BottomLeft) //left
			};
		}

		private void DebugArea(List<Rectangle> areas, Color clr) {
			foreach (Rectangle r in areas) {
				DebugArea(r, clr);
			}
		}

		private void DebugArea(Rectangle rect, Color clr) {
			SwinGame.FillRectangle(clr, rect);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;
using TaskForceUltra.src.GameModule;

namespace TaskForceUltra
{
	/// <summary>
	/// Keeps track of game entities to update and draw them 
	/// Entity handler is used by many other objects
	/// </summary>
	public class EntityHandler : IHandlesEntities
	{
		public List<Entity> EntityList { get; private set; }
		private Scoresheet scoresheet;

		public EntityHandler(Scoresheet scoresheet) {
			this.scoresheet = scoresheet;
			EntityList = new List<Entity>();
		}

		public void Update() {
			for (int i = EntityList.Count() - 1; i >= 0; --i) {
				EntityList[i].Update();

				if (EntityList[i].IsDead) {
					//score
					if (EntityList[i].KilledBy != Team.None) {
						scoresheet?.AddPoints(EntityList[i].KilledBy, EntityList[i].Mass);
					}

					//create debris
					List<LineSegment> lines = EntityList[i].DebrisLines;
					if (lines != null) {
						foreach (LineSegment l in lines) {
							Debris debris = new DebrisFactory().Create(l, EntityList[i].RealPos);
							Track(debris);
						}
					}

					Untrack(EntityList[i]);
				}
			}
		}

		public void Draw(Rectangle viewport) {
			//don't draw stuff unless it's within the view port
			foreach(Entity e in EntityList) {
				if (SwinGame.PointInRect(e.RealPos, viewport))
					e.Draw();
			}
		}

		public void Track(Entity entity) {
			if (entity != null)
				EntityList.Add(entity);
		}

		public void Untrack(Entity entity) {
			EntityList.Remove(entity);
		}

		/// <summary>
		/// returns how many AI ships are still alive
		/// </summary>
		public int AIShipCount() {
			int result = 0;

			foreach(AIShip aiShip in EntityList.OfType<AIShip>()) {
				result += 1;
			}
			return result;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// interface for tracking and untracking entities from an entity handler
	/// </summary>
	public interface IHandlesEntities
	{
		List<Entity> EntityList { get; }

		void Track(Entity entity);
		void Untrack(Entity entity);
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Handlers;
using TaskForceUltra.src.GameModule.Entities;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Handles collisions between collideable entities
	/// </summary>
	public class CollisionHandler
	{
		private Node quadTree;
		private IHandlesEntities entityHandler;

		public CollisionHandler(Rectangle playArea, IHandlesEntities entityHandler) {
			this.entityHandler = entityHandler;
			quadTree = new Node(null, playArea, 150);
		}

		public void Update() {
			RegisterAll();
			CollideEntities();

			quadTree.DebugDraw();
		}

		/// <summary>
		/// Register all entities being tracked by the passed in entity handler into the collision tree
		/// </summary>
		public void RegisterAll() {
			quadTree.Clear();
			foreach (ICollides c in entityHandler.EntityList.OfType<ICollides>()) {
				quadTree.Register(c);
			}
		}

		/// <summary>
		/// Run collision checking on all entities
		/// </summary>
		private void CollideEntities() {
			quadTree.CheckedList.Clear();
			
			foreach (ICollides self in entityHandler.EntityList.OfType<ICollides>()) {
				ICollides other = quadTree.CollidingWith(self);

				if (other != null) {
					self.ReactToCollision(other.Damage, other.Vel, other.Mass, other.Team, other is Ammo);
					other.ReactToCollision(self.Damage, self.Vel, self.Mass, self.Team, self is Ammo);
				}
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule
{
	public interface ICollides {
		Point2D RealPos { get; }
		Team Team { get; }
		Vector Vel { get; }
		int Mass { get; }
		int Damage { get; }
		List<LineSegment> BoundingBox { get; }

		void ReactToCollision(int dmg, Vector collidingVel, int collidingMass, Team collider, bool forceReaction = false);
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;

namespace TaskForceUltra.src.GameModule.Handlers
{
	/// <summary>
	/// a tree node for storing entities within a specified area
	/// </summary>
	public class Node
	{
		private Node parent;
		private Node[] childNodes;
		private Rectangle grid;
		private int minWidth;
		public List<ICollides> ICollidesList { get; private set; } // list of all collideables
		public List<ICollides> CheckedList { get; private set; } // list of collideables that have already been collision checked

		public Node(Node parent, Rectangle grid, int minWidth) {
			this.parent = parent;
			this.grid = grid;
			this.minWidth = minWidth;
			ICollidesList = new List<ICollides>();
			CheckedList = new List<ICollides>();

			if (Math.Abs(grid.Width) > minWidth)
				CreateChildren();
		}

		public void DebugDraw() {
			if (DebugMode.IsDebugging(Debugging.Nodes)) {
				if (childNodes != null) {
					foreach (Node n in childNodes) {
						n.DebugDraw();
					}
				}
				SwinGame.DrawRectangle(Color.Yellow, grid);

				foreach(ICollides c in ICollidesList) {
					SwinGame.FillRectangle(Color.Aqua, c.RealPos.X, c.RealPos.Y, 10, 10);
				}

				if (ICollidesList.Count > 0) {
					SwinGame.FillRectangle(SwinGame.RGBAColor(255, 255, 0, 100), grid);
				}
			}
		}

		/// <summary>
		/// Recursive initialisation of the tree structure
		/// </summary>
		private void CreateChildren() {
			childNodes = new Node[4];
			Rectangle[] grids = CreateGrids();

			for(int i=0; i<4; ++i) {
				childNodes[i] = new Node(this, grids[i], minWidth);
			}
		}

		/// <summary>
		/// Split current grid into 4 children grids
		/// </summary>
		/// <returns>4 rectangles</returns>
		private Rectangle[] CreateGrids() {
			Point2D gridCenter = SwinGame.PointAt(grid.X + (grid.Width / 2), grid.Y + (grid.Height / 2));

			return new Rectangle[4] {
				SwinGame.CreateRectangle(grid.TopLeft, gridCenter), //top left
				SwinGame.CreateRectangle(grid.CenterTop, grid.CenterRight), //top right
				SwinGame.CreateRectangle(gridCenter, grid.BottomRight), //bottom right
				SwinGame.CreateRectangle(grid.CenterLeft, grid.CenterBottom) //bottom left
			};
		}

		/// <summary>
		/// check if the passed in collideable is colliding with anything
		/// </summary>
		/// <returns>return what it is colliding w ith</returns>
		public ICollides CollidingWith(ICollides self) {
			Node n = FetchContaining(self);
			
			//guard
			if (n == null)
				return null;

			Node[] adjacentNodes = n.parent.childNodes;

			foreach (Node adjacentNode in adjacentNodes) {
				foreach (ICollides other in adjacentNode.ICollidesList) {
					if (self != other && self.Team != other.Team) {
						if (IsColliding(self.BoundingBox, other.BoundingBox)) {
							return other;
						}
					}
				}
			}
			
			return null;
		}

		/// <summary>
		/// Checks collision between line segments
		/// </summary>
		private bool IsColliding(List<LineSegment> bounds1, List<LineSegment> bounds2) {
			foreach(LineSegment l1 in bounds1) {
				foreach(LineSegment l2 in bounds2) {
					if (SwinGame.LineSegmentsIntersect(l1, l2))
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Register collidable to the right node
		/// </summary>
		public void Register(ICollides obj) {
			//guard
			if (obj == null)
				return;

			//end condition
			if (childNodes == null) {
				ICollidesList.Add(obj);
				return;
			}

			//find the node which contains the entiites position
			foreach(Node n in childNodes) {
				//only explore traverse relevant nodes
				if (n.ContainsPos(obj.RealPos)) {
					n.Register(obj);
				}
			}
		}

		/// <summary>
		/// remove the collideable entity from the tree
		/// </summary>
		/// <param name="obj">collideable object</param>
		public void Deregister(ICollides obj) {
			Node n = FetchContaining(obj);

			if (n != null)
				n.ICollidesList.Remove(obj);
		}

		/// <summary>
		/// clear the tree of all collideable entities
		/// </summary>
		public void Clear() {
			ICollidesList?.Clear();

			if (childNodes == null)
				return;

			foreach (Node n in childNodes) {
				n.Clear();
			}
		}

		/// <summary>
		/// Traverse the tree to fetch the node that contains the specified collideable object
		/// </summary>
		/// <param name="obj">collideable object</param>
		/// <returns>The containing node</returns>
		private Node FetchContaining(ICollides obj) {
			//guard
			if (obj == null)
				return null;

			//end condition (no more child nodes)
			if (childNodes == null) {
				if (ICollidesList.Contains(obj))
					return this;
				else return null;
			}
			else {
				//traverse
				foreach (Node n in childNodes) {
					if (n.ContainsPos(obj.RealPos))
						return n.FetchContaining(obj);
				}

				return null;
			}
		}

		/// <summary>
		/// Checks whether the specified point is within the node's area
		/// </summary>
		/// <param name="pos">An x,y point</param>
		/// <returns>true or false</returns>
		public bool ContainsPos(Point2D pos) {
			return pos.InRect(grid);
		}
	}
}

﻿using System;
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
			if (!cdHandler.IsOnCooldown()) {
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.AI
{
	/// <summary>
	/// AI entity interface for being controlled by an AI strategy
	/// </summary>
	public interface IAIEntity
	{
		bool IsDead { get; }
		Point2D RealPos { get; }
		Team Team { get; }
		Vector Vel { get; }
		float MaxVel { get; }

		void TurnTo(Vector targetDir, float turnStrength = 1);
		bool ShouldThrust(Vector targetDir);
		void Thrust(Vector vDir);
		void Fire();
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Entities;
using TaskForceUltra.src.GameModule.AI.strategies;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.AI
{
	/// <summary>
	/// base class for AI strategy
	/// </summary>
	public abstract class AIStrategy
	{
		protected IAIEntity controlled;
		protected Vector targetDir;
		protected CooldownHandler shootCooldown;

		public AIStrategy(IAIEntity controlled, int shootCd) {
			this.controlled = controlled;

			targetDir = Util.RandomUnitVector();
			shootCooldown = new CooldownHandler(shootCd * 1000);
		}

		public void Update() {
			if (controlled.IsDead)
				return;

			Shoot();
			ExecuteStrategy();
		}

		protected virtual void ExecuteStrategy() {
			if (controlled == null || controlled.IsDead)
				return;
		}

		protected void Shoot() {
			if (!shootCooldown.IsOnCooldown()) {
				controlled.Fire();
				shootCooldown.StartCooldown();
			}
		}
	}

	/// <summary>
	/// returns a randomised ai strategy based on some probability curve from a difficulty level
	/// </summary>	
	public class AIStrategyFactory
	{
		private int difficultyLevel;
		private int shootCooldown;

		public AIStrategyFactory(int difficultyLevel, int shootCooldown) {
			this.difficultyLevel = difficultyLevel;
			this.shootCooldown = shootCooldown;
		}

		public AIStrategy Create(IAIEntity aiEntity, IHandlesEntities entHandler) {
			//generate random number up to the difficulty level
			int n = Util.Rand(difficultyLevel);

			//return the hardest strategy that the number can get
			if (n < 10) {
				return new StaticStrategy(aiEntity, shootCooldown);
			}
			else if (n < 20) {
				return new ErraticStrategy(aiEntity, shootCooldown);
			}
			else {
				return new ChaseStrategy(aiEntity, entHandler, shootCooldown);
			}
		}

		public AIStrategy Create(IAIEntity aiEntity) {
			return Create(aiEntity, null);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Entities;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.AI.strategies
{
	/// <summary>
	/// chases ship's on the other team if they come within range of it
	/// </summary>
	public class ChaseStrategy : ErraticStrategy
	{
		private IHandlesEntities entHandler;
		private Ship target;
		private float agroRange;

		public ChaseStrategy(IAIEntity controlled, IHandlesEntities entHandler, int shootCooldown) : base(controlled, shootCooldown) {
			this.entHandler = entHandler;
			agroRange = SwinGame.ScreenWidth() / 1.5f;
		}

		protected override void ExecuteStrategy() {
			if (controlled == null || controlled.IsDead)
				return;

			target = FetchNearestTarget();

			//run erratic strategy if no target found
			if (target == null) {
				base.ExecuteStrategy();
			}
			//chase strategy
			else {
				//steering vector
				Vector DesiredVec = target.RealPos.Subtract(controlled.RealPos);
				DesiredVec = DesiredVec.LimitToMagnitude(controlled.MaxVel);
				Vector SteeringVec = DesiredVec.SubtractVector(controlled.Vel);
				targetDir = SteeringVec.UnitVector;

				//rotate
				controlled.TurnTo(targetDir);

				//thrust
				if (controlled.ShouldThrust(targetDir)) {
					Vector vDir = SwinGame.VectorTo(1, 0);
					controlled.Thrust(vDir);
				}
			}
		}

		/// <summary>
		/// Find nearest entity within agro range
		/// </summary>
		/// <returns>target ship</returns>
		private Ship FetchNearestTarget() {
			if (entHandler == null)
				return null;

			Ship result = null;
			float distanceToTarget = agroRange;

			foreach (Ship e in entHandler?.EntityList?.OfType<Ship>()) {
				float distanceToEntity = SwinGame.PointPointDistance(controlled.RealPos, e.RealPos);

				if (e.Team != controlled.Team && distanceToEntity < distanceToTarget) {
					result = e;
					distanceToTarget = distanceToEntity;
				}
			}

			return result;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Entities;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.AI.strategies
{
	/// <summary>
	/// Choose a direction once and keep thrusting in that direction until we die
	/// </summary>
	public class StaticStrategy : AIStrategy
	{
		public StaticStrategy(IAIEntity controlled, int shootCooldown) : base(controlled, shootCooldown) {
			targetDir = Util.RandomUnitVector();
		}

		protected override void ExecuteStrategy() {
			base.ExecuteStrategy();

			//rotate
			controlled.TurnTo(targetDir);

			//thrust
			if (controlled.ShouldThrust(targetDir)) {
				controlled.Thrust(SwinGame.VectorTo(1, 0));
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Entities;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.AI.strategies
{
	/// <summary>
	/// randomly change direction about every 5 seconds
	/// </summary>
	public class ErraticStrategy : AIStrategy
	{
		private CooldownHandler cdHandler;
		private int turnCooldown;

		public ErraticStrategy(IAIEntity controlled, int shootCooldown) : base(controlled, shootCooldown) {
			turnCooldown = 5000;

			cdHandler = new CooldownHandler(Util.Rand(turnCooldown));
			cdHandler.StartCooldown();
		}

		protected override void ExecuteStrategy() {
			base.ExecuteStrategy();

			//set new random vector and random timer threshhold
			if (!cdHandler.IsOnCooldown()) {
				Vector newDir = Util.RandomUnitVector();
				cdHandler.StartNewThreshhold(Util.Rand(turnCooldown));
			}

			//rotate
			controlled.TurnTo(targetDir);

			//thrust
			if (controlled.ShouldThrust(targetDir)) {
				Vector vDir = SwinGame.VectorTo(1, 0);
				controlled.Thrust(vDir);
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Commands;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Invoker object that listens for player input on the passed in action bindings
	/// and activates the relevant commands
	/// </summary>
	public class InputController
	{
		private IControllable controlled;
		private IActionBinding bindings;

		private ICommand forwardCommand;
		private ICommand backwardsCommand;
		private ICommand strafeLeftCommand;
		private ICommand strafeRightcommand;
		private ICommand turnLeftCommand;
		private ICommand turnRightCommand;
		private ICommand activatePowerupCommand;
		private ICommand shootCommand;

		public InputController(IControllable c, IActionBinding b)
		{
			controlled = c;
			bindings = b;

			CreateCommands();
		}

		public void Update() {
			HandleInput();
		}

		private void HandleInput() {
			//Movement
			if (bindings.Forward())
				forwardCommand.Execute();
			if (bindings.Backward())
				backwardsCommand.Execute();
			if (bindings.StrafeLeft())
				strafeLeftCommand.Execute();
			if (bindings.StrafeRight())
				strafeRightcommand.Execute();

			//Rotation
			if (bindings.TurnRight())
				turnRightCommand.Execute();
			if (bindings.TurnLeft())
				turnLeftCommand.Execute();

			//actions
			if (bindings.Shoot())
				shootCommand.Execute();
			if (bindings.ActivatePowerup())
				Console.WriteLine("activate powerup input received");
		}

		private void CreateCommands() {
			var commandFac = new GameCommandFactory(controlled);

			activatePowerupCommand = commandFac.Create(ShipAction.ActivatePowerup);
			shootCommand = commandFac.Create(ShipAction.Shoot);
			forwardCommand = commandFac.Create(ShipAction.Forward);
			backwardsCommand = commandFac.Create(ShipAction.Backward);
			strafeLeftCommand = commandFac.Create(ShipAction.StrafeLeft);
			strafeRightcommand = commandFac.Create(ShipAction.StrafeRight);
			turnLeftCommand = commandFac.Create(ShipAction.TurnLeft);
			turnRightCommand = commandFac.Create(ShipAction.TurnRight);
		}
	}


	/// <summary>
	/// Input Controller Factory
	/// </summary>
	public class InputControllerFactory
	{
		public InputController Create(IControllable controlled, ControllerType controller) {
			ActionBindingFactory actionBindingFac = new ActionBindingFactory(SwinGame.AppPath() + "\\resources\\data\\bindings");

			switch(controller) {
				case ControllerType.Player1:
					return new InputController(controlled, actionBindingFac.Create(ControllerType.Player1));
				case ControllerType.Player2:
					return new InputController(controlled, actionBindingFac.Create(ControllerType.Player2));
				case ControllerType.Player3:
					return new InputController(controlled, actionBindingFac.Create(ControllerType.Player3));
				case ControllerType.Player4:
					return new InputController(controlled, actionBindingFac.Create(ControllerType.Player4));
				default:
					return new InputController(controlled, actionBindingFac.Create(ControllerType.Player1));
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Receiver interface for a controllable game object
	/// </summary>
	public interface IControllable
	{
		void ForwardCommand();
		void BackwardCommand();
		void StrafeLeftCommand();
		void StrafeRightCommand();
		void TurnRightCommand();
		void TurnLeftCommand();
		void ShootCommand();
		void ActivatePowerupCommand();
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// action bindings interface
	/// </summary>
	public interface IActionBinding
	{
		bool ActivatePowerup();
		bool Backward();
		bool Forward();
		bool Shoot();
		bool StrafeLeft();
		bool StrafeRight();
		bool TurnLeft();
		bool TurnRight();
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Allows different player inputs to trigger the same ship action
	/// </summary>
	public class ActionBinding : IActionBinding
	{
		private Dictionary<ShipAction, KeyCode> bindings;

		public ActionBinding(Dictionary<ShipAction, KeyCode> bindings) {
			this.bindings = bindings;
		}

		//check action bindings to see if player wants to perform a particular ship action
		public bool Forward() {
			return SwinGame.KeyDown(bindings[ShipAction.Forward]);
		}
		public bool Backward() {
			return SwinGame.KeyDown(bindings[ShipAction.Backward]);

		}
		public bool StrafeLeft() {
			return SwinGame.KeyDown(bindings[ShipAction.StrafeLeft]);
		}
		public bool StrafeRight() {
			return SwinGame.KeyDown(bindings[ShipAction.StrafeRight]);
		}
		public bool TurnLeft() {
			return SwinGame.KeyDown(bindings[ShipAction.TurnLeft]);
		}
		public bool TurnRight() {
			return SwinGame.KeyDown(bindings[ShipAction.TurnRight]);
		}
		public bool Shoot() {
			return SwinGame.KeyDown(bindings[ShipAction.Shoot]);
		}
		public bool ActivatePowerup() {
			return SwinGame.KeyTyped(bindings[ShipAction.ActivatePowerup]);
		}
	}

	/// <summary>
	/// Action Binding Factory
	/// </summary>
	public class ActionBindingFactory
	{
		private string dirPath;

		public ActionBindingFactory(string dirPath) {
			this.dirPath = dirPath;
		}

		/// <summary>
		/// Returns the unique bindings for the specified player / controller
		/// </summary>
		/// <returns>Action bindings</returns>
		public IActionBinding Create(ControllerType controller) {
			switch(controller) {
				case ControllerType.Player1:
					return ReadActionBinding("\\Player1.json");
				case ControllerType.Player2:
					return ReadActionBinding("\\Player2.json");
				case ControllerType.Player3:
					return ReadActionBinding("\\Player3.json");
				case ControllerType.Player4:
					return ReadActionBinding("\\Player4.json");
				default:
					return ReadActionBinding("\\Player1.json");
			}
		}

		private IActionBinding ReadActionBinding(string fileName) {
			string filePath = dirPath + fileName;
			if (!File.Exists(filePath))
				return null;

			var result = new Dictionary<ShipAction, KeyCode>();
			JObject bindingsObj = Util.Deserialize(filePath);

			//iterate through binding settings in the json object
			foreach (JProperty binding in bindingsObj.GetValue("Bindings").OfType<JProperty>()) {
				Enum.TryParse(binding.Name, out ShipAction key);
				Enum.TryParse((string)binding.Value, out KeyCode value);

				result.Add(key, value);
			}

			return new ActionBinding(result);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class ActivatePowerupCommand : ICommand
	{
		IControllable controlled;

		public ActivatePowerupCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.ActivatePowerupCommand();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class BackwardCommand : ICommand
	{
		IControllable controlled;

		public BackwardCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.BackwardCommand();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class ForwardCommand : ICommand
	{
		IControllable controlled;

		public ForwardCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.ForwardCommand();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class StrafeRightCommand : ICommand
	{
		IControllable controlled;

		public StrafeRightCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.StrafeRightCommand();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class GameCommandFactory
	{
		private IControllable controlled;
		
		public GameCommandFactory(IControllable controlled) {
			this.controlled = controlled;
		}

		public ICommand Create(ShipAction action) {
			switch (action) {
				case ShipAction.Forward:
					return new ForwardCommand(controlled);
				case ShipAction.Backward:
					return new BackwardCommand(controlled);
				case ShipAction.StrafeLeft:
					return new StrafeLeftCommand(controlled);
				case ShipAction.StrafeRight:
					return new StrafeRightCommand(controlled);
				case ShipAction.TurnLeft:
					return new TurnLeftCommand(controlled);
				case ShipAction.TurnRight:
					return new TurnRightCommand(controlled);
				case ShipAction.ActivatePowerup:
					return new ActivatePowerupCommand(controlled);
				case ShipAction.Shoot:
					return new ShootCommand(controlled);
				default:
					return null;
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class TurnLeftCommand : ICommand
	{
		IControllable controlled;

		public TurnLeftCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.TurnLeftCommand();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class StrafeLeftCommand : ICommand
	{
		IControllable controlled;

		public StrafeLeftCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.StrafeLeftCommand();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class ShootCommand : ICommand
	{
		IControllable controlled;

		public ShootCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.ShootCommand();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class TurnRightCommand : ICommand
	{
		IControllable controlled;

		public TurnRightCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.TurnRightCommand();
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;


namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// base class manages the position of game objects
	/// </summary>
	public abstract class PositionedObject
	{
		protected Point2D refPos;
		protected Point2D offsetPos;
		public Point2D RealPos { get { return refPos.Add(offsetPos); } }

		public PositionedObject(Point2D refPos, Point2D offsetPos) {
			this.refPos = refPos;
			this.offsetPos = offsetPos;
		}
		
		public virtual void TeleportTo(Point2D target) {
			refPos = target;
		}

		/// <summary>
		/// Returns whether the object is on the screen or not
		/// </summary>
		protected bool IsOnScreen() {
			Rectangle cameraBox = SwinGame.CreateRectangle(Camera.CameraPos(), SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
			return SwinGame.PointInRect(RealPos, cameraBox);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Base entity object handles object shape, team, health, state
	/// </summary>
	public abstract class Entity : PositionedObject
	{
		public string Id { get; private set; }
		public Team Team { get; protected set; }
		public string FilePath { get; private set; }

		protected int baseHealth;
		protected int health;
		public float Condition {
			get { return (health / baseHealth); }
		}
		public bool IsDead { get; protected set; }
		public Team KilledBy { get; protected set; }

		public Shape Shape { get; private set; }
		public virtual List<LineSegment> DebrisLines { get { return Shape.GetLines(); } }
		protected List<Color> colors;
		protected int colorIndex;
		
		public List<LineSegment> BoundingBox { get { return Shape?.BoundingBox; } }
		public virtual int Mass {
			get	{
				if (Shape != null)
					return Shape.Mass;
				else return 1;
			}
		}

		public Entity(
			string id, string filePath, Point2D refPos, Point2D offsetPos,
			Shape shape, List<Color> colors, int health, Team team
		) : base(refPos, offsetPos)
		{
			Id = id;
			Team = team;
			FilePath = filePath;
			baseHealth = this.health = health;
			Shape = shape;
			this.colors = colors;
			colorIndex = 0;
			IsDead = false;
		}

		public virtual void Update() {
		}

		public virtual void Draw() {
			Shape?.Draw(colors[colorIndex]);
		}

		/// <summary>
		/// Teleport Entity to the target position
		/// </summary>
		/// <param name="target">x, y position</param>
		public override void TeleportTo(Point2D target) {
			base.TeleportTo(target);
			Shape?.TeleportTo(target);
		}

		/// <summary>
		/// Kill the entity and record which team scored the skill
		/// </summary>
		/// <param name="killer">team which scored killing blow</param>
		public virtual void Kill(Team killer) {
			IsDead = true;
			KilledBy = killer;
		}

		/// <summary>
		/// Debugging visuals
		/// </summary>
		protected virtual void Debug() {
			Shape?.Debug(Color.Red);
			SwinGame.FillRectangle(Color.Blue, refPos.X, refPos.Y, 5, 5);
		}
	}
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaskForceUltra.src.GameModule;

namespace TaskForceUltra
{
	/// <summary>
	/// Manages the shape of the parent object as a bunch of line segments
	/// </summary>
	public class Shape
	{
		private List<LineSegment> shape;
		private Point2D pos;
		public List<LineSegment> BoundingBox { get; private set; }
		public int Mass {
			get { return shape.Count; }
		}

		public Shape(List<LineSegment> shape, List<LineSegment> boundingBox, Point2D offsetPos) {
			this.shape = shape;
			BoundingBox = boundingBox;
			pos = offsetPos;
		}

		public void TeleportTo(Point2D target) {
			Vector moveBy = target.Add(pos.Multiply(-1));
			Move(moveBy);
			pos = target;
		}

		public void Move(Vector v) {
			BoundingBox = BoundingBox?.Move(v);
			pos += v;
		}

		public void Rotate(double theta) {
			shape = shape?.Rotate(SwinGame.PointAt(0, 0), theta);
			BoundingBox = BoundingBox?.Rotate(pos, theta);
		}

		public void Draw(Color clr) {
			foreach (LineSegment l in shape) {
				l.Draw(clr, pos);
			}
		}

		public void Debug(Color clr) {
			if (BoundingBox == null)
				return;

			foreach (LineSegment l in BoundingBox) {
				SwinGame.DrawLine(clr, l);
			}
		}

		/// <summary>
		/// Returns line segments that it is managing
		/// </summary>
		/// <returns>List of line segments</returns>
		public List<LineSegment> GetLines() {
			return shape;
		}
	}

	/// <summary>
	/// Shape Factory
	/// </summary>
	public class ShapeFactory
	{
		public Shape CreateCircleApprox(float length, int edges) {
			MinMax<float> angleRange = new MinMax<float>(0.3f, 1.5f);
			edges = Math.Max(edges, 3);
			float[] angles = new float[edges];

			//use random angles
			angles = BuildCircleApproxAngles(edges, angleRange);

			//create lines
			List<LineSegment> lines = new List<LineSegment>();
			Point2D lastPoint = SwinGame.PointAt(0, 0);
			float totalAngle = 0;
			foreach (float angle in angles) {
				totalAngle += angle;
				Vector vec = SwinGame.VectorFromAngle(totalAngle, length);
				lines.Add(SwinGame.CreateLine(lastPoint, lastPoint + vec));
				lastPoint = lines.Last().EndPoint;
			}
			lines.Add(SwinGame.CreateLine(lastPoint, SwinGame.PointAt(0, 0)));

			//bounding box
			List<LineSegment> boundingBox = CreateBoundingBox(lines);
			Shape shape = new Shape(lines, boundingBox, SwinGame.PointAt(0, 0));

			return shape;
		}

		private float[] BuildCircleApproxAngles(int edges, MinMax<float> angleRange) {
			float totalAngle = 0;
			float[] result = new float[edges];

			for (int i = 0; i < edges-1; i++) {
				float angle = (360 / edges) * Util.RandomInRange(angleRange);

				//check that we are not exceeding 360
				angle = totalAngle + angle < 360 ? angle : 360 - totalAngle;
				if (angle <= 0)
					break;
				result[i] = angle;
				totalAngle += angle;
			}

			if (totalAngle < 360)
				result[edges-1] = 360 - totalAngle;

			return result;
		}

		public Shape Create(JObject shapeObj, float s, Point2D offsetPos) {
			if (shapeObj == null)
				return null;

			float scale = (s == 0 ? 0.1f : s); //don't allow 0 scale
			int lineLength = (int)(10 * scale);

			List<LineSegment> shape = new List<LineSegment>();
			List<LineSegment> boundingBox = new List<LineSegment>();

			JArray linesObj = shapeObj.Value<JArray>("lines");
			JArray boxesObj = shapeObj.Value<JArray>("boxes");
			JArray trianglesObj = shapeObj.Value<JArray>("triangles");

			AddLines(shape, linesObj, lineLength);
			AddBoxes(shape, boxesObj, lineLength);
			AddTriangles(shape, trianglesObj, lineLength);
			boundingBox = CreateBoundingBox(shape);

			shape = shape.Move(offsetPos);
			shape = shape.Move(SwinGame.PointAt(-lineLength / 2, -lineLength / 2));

			return new Shape(shape, boundingBox, offsetPos);
		}

		//Methods for deserialising lines, boxes, triangles into line segments
		private void AddLines(List<LineSegment> shape, JArray linesObj, int lineLength) {
			if (linesObj == null)
				return;

			List<LineSegment> lines = linesObj.ToObject<List<LineSegment>>();

			for (int i = 0; i < lines.Count; ++i) {
				shape.Add(lines[i].Multiply(lineLength));
			}
		}

		private void AddBoxes(List<LineSegment> shape, JArray boxesObj, int lineLength) {
			if (boxesObj == null)
				return;

			List<Point2D> boxes = boxesObj.ToObject<List<Point2D>>();
			foreach (Point2D p in boxes) {
				Point2D[] corners = BoxCorners(p, lineLength);
				shape.Add(SwinGame.CreateLine(corners[0], corners[1]));
				shape.Add(SwinGame.CreateLine(corners[1], corners[2]));
				shape.Add(SwinGame.CreateLine(corners[2], corners[3]));
				shape.Add(SwinGame.CreateLine(corners[3], corners[0]));
			}
		}

		private void AddTriangles(List<LineSegment> shape, JArray trianglesObj, int lineLength) {
			if (trianglesObj == null)
				return;

			List<Point2D> triangles = trianglesObj.ToObject<List<Point2D>>();

			foreach (Point2D p in triangles) {
				Point2D[] corners = BoxCorners(p, lineLength);
				LineSegment bottom = SwinGame.CreateLine(corners[3], corners[2]);
				LineSegment left = SwinGame.CreateLine(corners[2], SwinGame.LineMidPoint(SwinGame.CreateLine(corners[0], corners[1])));
				LineSegment right = SwinGame.CreateLine(corners[3], SwinGame.LineMidPoint(SwinGame.CreateLine(corners[0], corners[1])));

				shape.Add(bottom);
				shape.Add(left);
				shape.Add(right);
			}
		}

		private Point2D[] BoxCorners(Point2D pos, float length) {
			return new Point2D[4]
			{
				pos.Multiply(length),
				SwinGame.PointAt(pos.Multiply(length).X + length, pos.Multiply(length).Y),
				SwinGame.PointAt(pos.Multiply(length).X + length, pos.Multiply(length).Y + length),
				SwinGame.PointAt(pos.Multiply(length).X, pos.Multiply(length).Y + length)
			};
		}

		//build bounding box for collision mask
		private List<LineSegment> CreateBoundingBox(List<LineSegment> lines) {
			float xMin = 0, xMax = 0, yMin = 0, yMax = 0;

			foreach (LineSegment l in lines) {
				FindFromPoint2D(l.StartPoint);
				FindFromPoint2D(l.EndPoint);
			}

			void FindFromPoint2D(Point2D linePos) {
				if (linePos.X < xMin)
					xMin = linePos.X;
				if (linePos.X > xMax)
					xMax = linePos.X;
				if (linePos.Y < yMin)
					yMin = linePos.Y;
				if (linePos.Y > yMax)
					yMax = linePos.Y;
			}

			return new List<LineSegment>() {
				SwinGame.CreateLine(xMin, yMin, xMax, yMin),
				SwinGame.CreateLine(xMax, yMin, xMax, yMax),
				SwinGame.CreateLine(xMax, yMax, xMin, yMax),
				SwinGame.CreateLine(xMin, yMax, xMin, yMin)
			};
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Something that moves around
	/// Handles movement logic
	/// </summary>
	public abstract class Mover : Entity
	{
		public Vector Vel { get; protected set; }

		public double theta; //radians
		public Vector Dir { get; protected set; }

		protected BoundaryStrategy boundaryStrat { get; private set; }

		private bool isOptimisedUpdate; // if true, do not update the object if it is off screen

		public Mover(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int health, Vector vel, Vector dir, BoundaryStrategy boundaryStrat,
			Team team, bool optimiseMe = false
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, team)
		{
			this.boundaryStrat = boundaryStrat;
			Vel = vel;
			Dir = dir;
			theta = 0;

			isOptimisedUpdate = optimiseMe;
		}

		public override void Update() {
			if (isOptimisedUpdate && !IsOnScreen())
				return;

			base.Update();
			Move();
			Rotate();
			boundaryStrat?.Run(this);
		}

		/// <summary>
		/// Automatically move based on whatever the current velocity is
		/// </summary>
		protected void Move() {
			refPos += Vel;
			Shape?.Move(Vel);
		}

		/// <summary>
		/// Automatically turn based on whatever theta has been set
		/// </summary>
		protected void Rotate() {
			Dir = Dir.Rotate(theta);
			Shape?.Rotate(theta);
			offsetPos = offsetPos.Rotate(SwinGame.PointAt(0,0), theta);
			theta = 0;
		}

		protected override void Debug() {
			SwinGame.DrawLine(Color.Blue, SwinGame.LineFromVector(Vel));
			base.Debug();
		}

		/// <summary>
		/// checks if the difference between the target direction and the current direction is small enough to start thrusting forward
		/// </summary>
		public bool ShouldThrust(Vector targetDir) {
			return Math.Abs(Dir.AngleTo(targetDir)) < 45;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaskForceUltra.src.GameModule.Entities;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.AI;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Spaceship object that has engines, tools, emitters
	/// </summary>
	public abstract class Ship : Mover, ICollides
	{
		private List<Component> componentList;

		protected int hurtThreshhold;
		protected Timer hurtTimer;
		protected bool isHurting;

		public float Accel {
			get {
				float result = 0;
				foreach(Engine e in componentList?.OfType<Engine>()) {
					result += e.Thrust;
				}
				return result;
			}
		}

		public float MaxVel {
			get {
				float result = 0;
				foreach (Engine e in componentList?.OfType<Engine>()) {
					result = e.MaxVel > result ? e.MaxVel : result;
				}
				return result;
			}
		}

		public int Damage { get; private set; }

		public override int Mass {
			get { return (base.Mass + componentList.Mass()); }
		}

		public Ship(
			string id, string filePath, Point2D refPos, Point2D offsetPos,
			Shape shape, List<Color> colors, int health, Vector vel, Vector dir, int threshhold,
			BoundaryStrategy boundaryStrat, Team team, List<Component> components
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, boundaryStrat, team)
		{
			componentList = components;
			Damage = 1;
			hurtTimer = SwinGame.CreateTimer();
			isHurting = false;
			hurtThreshhold = threshhold;
		}

		public override void Update() {
			base.Update();
			componentList?.Update();

			HandleHurtingState();
			TeleportTo(RealPos);
		}

		/// <summary>
		/// Handles damage timeouts to prevent things from losing all their health really quickly
		/// </summary>
		private void HandleHurtingState() {
			if (isHurting && hurtTimer.Ticks > hurtThreshhold) {
				hurtTimer.Stop();
				isHurting = false;
				colorIndex = 0;
			}
			else if (isHurting) {
				colorIndex = Util.Rand(colors.Count);
			}
		}

		public override void Draw() {
			base.Draw();
			componentList?.Draw();

			DebugDraw();
		}

		/// <summary>
		/// Object's reaction to being collided with
		/// </summary>
		/// <param name="dmg">incoming damage</param>
		/// <param name="collidingVel">velocity of other object</param>
		/// <param name="collidingMass">mass of other object</param>
		/// <param name="collider">team of other object</param>
		/// <param name="forceReaction">opt to bypass hurting timeout</param>
		public virtual void ReactToCollision(int dmg, Vector collidingVel, int collidingMass, Team collidingTeam, bool forceReaction = false) {
			if (!isHurting || forceReaction) {
				isHurting = true;
				hurtTimer.Start();

				health -= dmg;
				float velTransferMod = ((float)collidingMass / (float)Mass);
				Vel = Vel.AddVector(collidingVel.Multiply(velTransferMod));
				Vel = Vel.LimitToMagnitude(MaxVel);
			}

			if (health <= 0)
				Kill(collidingTeam);
		}


		//////////////////////
		// MOVEMENT API
		//////////////////////
		/// <summary>
		/// Ask ship engines to thrust ship along a passed in vector direction
		/// magnitude of vector direction determines thrust scaling
		/// </summary>
		public void Thrust(Vector vDir) {
			vDir = vDir.Rotate(Dir.Angle * (Math.PI / 180));

			foreach (Engine e in componentList?.OfType<Engine>()) {
				Vel = e.ApplyForce(vDir, Vel, Mass);
				componentList.SetVel(Vel);
			}
		}

		/// <summary>
		/// Ask engines to turn the ship based on a positive or negative value
		/// </summary>
		protected void Turn(float turnStrength) {
			foreach (Engine e in componentList?.OfType<Engine>()) {
				theta = e.Turn(turnStrength);
				componentList.Turn(theta);
			}
		}

		/// <summary>
		/// Turn the ship to a specified vector
		/// </summary>
		/// <param name="targetDir">vector to turn to</param>
		/// <param name="turnStrength">opt to modify turn strength</param>
		public void TurnTo(Vector targetDir, float turnStrength = 1) {
			//turn towards the target direction as much as our engines will allow us
			double desiredTheta = Dir.AngleTo(targetDir) * Math.PI/180;

			foreach (Engine e in componentList?.OfType<Engine>()) {
				double engineTheta = e.Turn(turnStrength);
				theta += engineTheta;
			}

			theta *= desiredTheta.GetSign();
			if (theta <= 0) {
				theta = theta.Clamp(desiredTheta, 0);
			} else {
				theta = theta.Clamp(0, desiredTheta);
			}

			componentList.Turn(theta);
		}

		/// <summary>
		/// Activate a specific tool
		/// </summary>
		/// <param name="toolId">tool id to activate</param>
		public void Fire(string toolId) {
			foreach (Tool t in componentList?.OfType<Tool>()) {
				if (t.Id == toolId)
					t.Activate();
			}
		}

		/// <summary>
		/// Activate all tools
		/// </summary>
		public void Fire() {
			foreach (Tool t in componentList?.OfType<Tool>()) {
				t.Activate();
			}
		}

		/// <summary>
		/// Teleport the ship to the target position
		/// </summary>
		public override void TeleportTo(Point2D target) {
			base.TeleportTo(target);
			componentList?.TeleportTo(target);
		}

		public void SetTeam(Team team) {
			Team = team;
		}

		protected void DebugDraw() {
			if (DebugMode.IsDebugging(Debugging.Ship)) {
				Debug();
			}
		}
	}


	/// <summary>
	/// Ship Factory
	/// </summary>
	public class ShipFactory
	{
		public Dictionary<string, Shape> ShapeRegistry { get; private set; }
		public Dictionary<string, string> FileRegistry { get; private set; }
		private string[] fileList;

		private string dirPath;

		public ShipFactory(string dirPath) {
			this.dirPath = dirPath;
			fileList = Directory.GetFiles(dirPath);
			ShapeRegistry = new Dictionary<string, Shape>();
			FileRegistry = new Dictionary<string, string>();
			RegisterShips();
		}

		/// <summary>
		/// Register ships from file
		/// </summary>
		public void RegisterShips() {
			BuildFileRegistry();
			BuildShapeRegistry();
		}

		/// <summary>
		/// Ship id, filename pairs
		/// </summary>
		private void BuildFileRegistry() {
			try {
				FileRegistry.Clear();

				foreach (string file in fileList) {
					JObject obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(file));
					string id = obj?.Value<string>("id");
					if (id != null && !FileRegistry.ContainsKey(id))
						FileRegistry.Add(id, file);
				}
			} catch (Exception e) {
				Console.WriteLine($"invalid file {e}");
			}
		}

		/// <summary>
		/// ship id, ship shape pairs
		/// useful if we only want to draw the shapes
		/// </summary>
		private void BuildShapeRegistry() {
			try {
				ShapeRegistry.Clear();

				foreach (string file in fileList) {
					JObject obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(file));
					string id = obj?.Value<string>("id");

					//build the ship's shape
					ShapeFactory shapeFac = new ShapeFactory();
					float scale = obj.Value<float>("scale");
					Shape shape = shapeFac.Create(obj.Value<JObject>("shape"), scale, SwinGame.PointAt(0, 0)); //done

					if (id != null && shape != null && !ShapeRegistry.ContainsKey(id))
						ShapeRegistry.Add(id, shape);
				}
			} catch (Exception e) {
				Console.WriteLine($"invalid file {e}");
			}
		}

		/// <summary>
		/// Return the file path of the specified ship id
		/// </summary>
		/// <param name="id">ship id</param>
		/// <returns>ship file path</returns>
		public string FetchShipPath(string id) {
			return FileRegistry?[id];
		}

		private List<Component> BuildComponents(JArray enginesObj, JArray toolsObj, JArray emittersObj,
			IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offset, float mod = 1) {
			List<Component> result = new List<Component>();

			result.AddRange(new EngineFactory().CreateList(enginesObj, entHandler, boundaryStrat, team, offset, mod));
			result.AddRange(new ToolFactory().CreateList(toolsObj, entHandler, boundaryStrat, team, offset, mod));
			result.AddRange(new EmitterFactory().CreateList(emittersObj, entHandler, boundaryStrat, team, offset, mod=1));

			return result;
		}

		private PlayerShip CreatePlayerShip(string shipId, Point2D pos, BoundaryStrategy boundaryStrat, ControllerType controller, IHandlesEntities entHandler) {
			JObject obj = Util.Deserialize(FileRegistry[shipId]);

			int health = obj.Value<int>("health");
			List<Color> shipColors = new List<Color> { Util.GetRGBColor(obj.GetValue("color")), Color.Yellow, Color.White, Color.Red };
			float scale = obj.Value<float>("scale");
			JArray enginesObj = obj.Value<JArray>("engines");
			JArray toolsObj = obj.Value<JArray>("tools");
			JArray emittersObj = obj.Value<JArray>("emitters");
			JObject shapeObj = obj.Value<JObject>("shape");

			Team team = (Team)(int)controller;
			Point2D offset = SwinGame.PointAt(0, 0);

			//shape
			Shape shape = new ShapeFactory().Create(shapeObj, scale, SwinGame.PointAt(0, 0));
			//shape.TeleportTo(pos);

			//component
			List<Component> components = BuildComponents(enginesObj, toolsObj, emittersObj, entHandler, boundaryStrat, team, offset);

			PlayerShip result = new PlayerShip(shipId, FileRegistry[shipId], pos, SwinGame.PointAt(0, 0), shape, shipColors,
				health, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1), boundaryStrat, team, components);

			result.TeleportTo(pos);
			return result;
		}

		private AIShip CreateAIShip(string shipId, Point2D pos,BoundaryStrategy boundaryStrat, Difficulty diff, IHandlesEntities entHandler) {
			AIStrategyFactory strategyFac = new AIStrategyFactory(diff.DifficultyLevel, diff.ShootCooldown);

			JObject obj = Util.Deserialize(FileRegistry[shipId]);

			int health = obj.Value<int>("health");
			List<Color> shipColors = new List<Color> { Color.Crimson, Color.Yellow, Color.White, Color.Red };
			float scale = obj.Value<float>("scale");
			JArray enginesObj = obj.Value<JArray>("engines");
			JArray toolsObj = obj.Value<JArray>("tools");
			JArray emittersObj = obj.Value<JArray>("emitters");
			JObject shapeObj = obj.Value<JObject>("shape");

			Team team = Team.Computer;
			Point2D offset = SwinGame.PointAt(0, 0);

			//shape
			Shape shape = new ShapeFactory().Create(shapeObj, scale, SwinGame.PointAt(0, 0));
			shape.TeleportTo(pos);

			//components
			List<Component> components = BuildComponents(enginesObj, toolsObj, emittersObj, entHandler, boundaryStrat, team, offset, diff.AIMod);

			//build and return ship
			AIShip result = new AIShip(shipId, FileRegistry[shipId], pos, SwinGame.PointAt(0, 0), shape, shipColors,
				health, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1), boundaryStrat, team, components);

			//create strategy
			AIStrategy aiStrat = strategyFac.Create((IAIEntity)result, entHandler);
			result.AIStrategy = aiStrat;

			result.TeleportTo(pos);
			return result;
		}

		/// <summary>
		/// Create specified ship
		/// </summary>
		/// <param name="shipId">ship id</param>
		/// <param name="pos">spawn position</param>
		/// <param name="boundaryStrat">behaviour at the play area boundary</param>
		/// <param name="controller">computer/players</param>
		/// <param name="diff">difficulty setting</param>
		/// <param name="entHandler">entity handler</param>
		/// <returns>player ship or ai ship depending on controller</returns>
		public Ship Create(string shipId, Point2D pos, BoundaryStrategy boundaryStrat, ControllerType controller, Difficulty diff, IHandlesEntities entHandler) {
			if (!FileRegistry.ContainsKey(shipId))
				return null;

			switch(controller) {
				case ControllerType.Computer:
					return CreateAIShip(shipId, pos, boundaryStrat, diff, entHandler);
				case ControllerType.Player1:
				case ControllerType.Player2:
				case ControllerType.Player3:
				case ControllerType.Player4:
					return CreatePlayerShip(shipId, pos, boundaryStrat, controller, entHandler);
				default: return null;
			}
		}

		/// <summary>
		/// Create a random ship!
		/// </summary>
		/// <returns>a ship</returns>
		public Ship CreateRandomShip(Point2D pos, BoundaryStrategy boundaryStrat, ControllerType controller, Difficulty diff, IHandlesEntities entHandler) {
			int i = Util.Rand(FileRegistry.Count);
			string randomShipId = FileRegistry.ElementAt(i).Key;
			return Create(randomShipId, pos, boundaryStrat, controller, diff, entHandler);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Player ship that can be controlled by an input controller
	/// </summary>
	public class PlayerShip : Ship, IControllable
	{
		public PlayerShip(
			string id, string filePath, Point2D refPos, Point2D offsetPos,
			Shape shape, List<Color> colors, int health, Vector vel, Vector dir,
			BoundaryStrategy boundaryStrat, Team team, List<Component> components
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, 2000, boundaryStrat, team, components)
		{
		}

		public override void ReactToCollision(int dmg, Vector collidingVel, int collidingMass, Team collider, bool forceReaction = false) {
			forceReaction = false;
			base.ReactToCollision(dmg, collidingVel, collidingMass, collider, forceReaction);
		}

		/// <summary>
		/// receiver command implementations
		/// </summary>
		public void ForwardCommand() {
			Thrust(SwinGame.VectorTo(1, 0));
		}

		public void BackwardCommand() {
			Thrust(SwinGame.VectorTo(-1, 0));
		}

		public void StrafeLeftCommand() {
			Thrust(SwinGame.VectorTo(0, -1));
		}

		public void StrafeRightCommand() {
			Thrust(SwinGame.VectorTo(0, 1));
		}

		public void TurnLeftCommand() {
			Turn(-1);
		}

		public void TurnRightCommand() {
			Turn(1);
		}

		public void ActivatePowerupCommand() {
			//TODO need playership inventory system
		}

		public void ShootCommand() {
			Fire();
		}
	}
}

﻿using System;
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Base class for ship components
	/// </summary>
	public abstract class Component : Mover
	{
		protected List<Component> childComponents;

		public override int Mass {
			get {
				return (base.Mass + childComponents.Mass()); }
		}
		public override List<LineSegment> DebrisLines { get { return null; } }

		public Component(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape, List<Color> colors,
			int health, Vector vel, Vector dir, BoundaryStrategy boundaryStrat, Team team, bool optimiseMe = false
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, boundaryStrat, team, optimiseMe)
		{
			childComponents = new List<Component>();
		}

		public override void Update() {
			base.Update();
			childComponents?.Update();
		}

		public override void Draw() {
			base.Draw();
			childComponents?.Draw();

			DebugDraw();
		}

		protected virtual void DebugDraw() {
			if (DebugMode.IsDebugging(Debugging.Component))
				Debug();
		}

		/// <summary>
		/// Ask component to move with specified velocity vector
		/// </summary>
		/// <param name="v">velocity vector</param>
		public void SetVel(Vector v) {
			Vel = v;
			childComponents?.SetVel(v);
		}

		/// <summary>
		/// Ask component to turn by specified radians
		/// </summary>
		/// <param name="theta"></param>
		public void Turn(double theta) {
			this.theta = theta;
			childComponents?.Turn(theta);
		}

		public override void Kill(Team killer) {
			base.Kill(Team.None);
		}
	}

	/// <summary>
	/// Component Factory
	/// </summary>
	public abstract class ComponentFactory
	{
		/// <summary>
		/// Create component from a filepath
		/// </summary>
		/// <param name="refObj">Json object containing the filepath</param>
		/// <param name="entHandler">entity handler</param>
		/// <param name="boundaryStrat">play area boundary behaviour</param>
		/// <param name="parentPos">position of object component is attached to</param>
		/// <param name="mod">modifier</param>
		/// <returns></returns>
		public virtual Component CreateFromReference(JObject refObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod = 1) {
			string path = SwinGame.AppPath() + refObj.Value<string>("path");

			//check that the path is valid
			if (!File.Exists(path)) {
				Console.WriteLine($"INVALID filepath: {path}");
				return null;
			}

			//load the full JObject from path
			refObj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
			return Create(refObj, path, entHandler, boundaryStrat, team, parentPos, mod);
		}

		public abstract Component Create(JObject compObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offsetPos, float mod = 1);

		public virtual List<Component> CreateList(JArray compObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod = 1) {
			List<Component> result = new List<Component>();

			foreach(JObject obj in compObj) {
				result.Add(CreateFromReference(obj, entHandler, boundaryStrat, team, parentPos));
			}

			return result;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// An emitter that creates particles
	/// </summary>
	public class Emitter : Component
	{
		public Particle Particle { get { return childComponents.OfType<Particle>().First(); } }
		private IHandlesEntities entHandler;

		private float cooldownRate;
		private CooldownHandler cdHandler;

		public Emitter(
			string id, string filePath, Point2D refPos, Point2D offsetPos,
			Vector vel, Vector dir, BoundaryStrategy boundaryStrat, Team team,
			float cooldownRate, List<Component> children, IHandlesEntities entHandler
		) : base(id, filePath, refPos, offsetPos, null, null, 1, vel, dir, boundaryStrat, team)
		{
			childComponents = children;
			this.cooldownRate = cooldownRate;
			cdHandler = new CooldownHandler(1000 / cooldownRate);
			this.entHandler = entHandler;
		}

		/// <summary>
		/// Create, initialise, and track new particle
		/// </summary>
		public void Activate() {
			if (!cdHandler.IsOnCooldown()) {
				JObject particleObj = Util.Deserialize(Particle.FilePath);

				Particle newParticle = new ParticleFactory().Create(particleObj, FilePath, entHandler, boundaryStrat, Team, offsetPos) as Particle;

				newParticle.Init(refPos, Dir);
				entHandler.Track(newParticle);
				cdHandler.StartCooldown();
			}
		}

		public override void Update() {
			base.Update();
		}
	}

	/// <summary>
	/// Emitter Factory
	/// </summary>
	public class EmitterFactory : ComponentFactory
	{
		public override Component CreateFromReference(JObject emitterObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod =1) {
			Point2D offsetPos = emitterObj["pos"].ToObject<Point2D>().Multiply(10);
			offsetPos = offsetPos.Add(parentPos);

			return base.CreateFromReference(emitterObj, entHandler, boundaryStrat, team, offsetPos);
		}

		public override Component Create(JObject emitterObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offsetPos, float mod =1) {
			string id = emitterObj.Value<string>("id");
			float cooldownRate = emitterObj.Value<float>("rate");

			JObject particleObj = emitterObj.Value<JObject>("particle");
			Component particle = new ParticleFactory().CreateFromReference(particleObj, entHandler, boundaryStrat, team, offsetPos);

			return new Emitter(id, path, SwinGame.PointAt(0, 0), offsetPos, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1),
				boundaryStrat, team, cooldownRate, new List<Component>() { particle }, entHandler);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Particle object that doesn't really interact with anything else
	/// Impacts performance if too many of these things are on screen
	/// </summary>
	public class Particle : Component
	{
		private MinMax<float> lifetimeRange;
		private MinMax<float> velRange;
		private MinMax<float> turnRateRange;

		private float friction;
		private float thrustForce;
		private float turnRate;
		private float lifetime;

		private CooldownHandler cdHandler;

		public Particle(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, float friction, MinMax<float> lifetimeRange, MinMax<float> velRange,
			MinMax<float> turnRateRange, BoundaryStrategy boundaryStrat, Team team
		) : base(id, filePath, refPos, offsetPos, shape, colors, 1, SwinGame.VectorTo(0,0), SwinGame.VectorTo(0, -1), boundaryStrat, team, true)
		{
			this.friction = friction;
			this.lifetimeRange = lifetimeRange;
			this.velRange = velRange;
			this.turnRateRange = turnRateRange;

			thrustForce = 0;
			turnRate = 0;
			lifetime = 0;
		}

		public override void Update() {
			if (cdHandler != null) {
				if (cdHandler.IsOnCooldown()) {
					Vel = Vel.Multiply(friction);
					turnRate *= friction;
					theta = turnRate;

					colorIndex = SwinGame.Rnd(colors.Count);
				}
				else Kill(Team.None);
			}

			base.Update();
		}

		public override void Draw() {
			if (cdHandler != null)
				base.Draw();

			DebugDraw();
		}

		/// <summary>
		/// Initialise the particle
		/// </summary>
		/// <param name="pos">spawning position</param>
		/// <param name="dir">spawning direction</param>
		public void Init(Point2D pos, Vector dir) {
			thrustForce = Util.RandomInRange(velRange);
			turnRate = Util.RandomInRange(turnRateRange);
			lifetime = Util.RandomInRange(lifetimeRange);
			cdHandler = new CooldownHandler(lifetime * 1000);
			cdHandler.StartCooldown();

			TeleportTo(pos);
			double theta = Dir.AngleTo(dir) * Math.PI / 180;
			this.theta = theta;

			Vector deltaV = dir.Multiply(-thrustForce);
			Vel = (Vel.AddVector(deltaV)).LimitToMagnitude(thrustForce);
		}

		protected override void DebugDraw() {
			if (DebugMode.IsDebugging(Debugging.Particle))
				Debug();
		}
	}


	/// <summary>
	/// Particle Factory
	/// </summary>
	public class ParticleFactory : ComponentFactory
	{
		public override Component CreateFromReference(JObject particleObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod =1) {
			string path = SwinGame.AppPath() + particleObj.Value<string>("path");

			return base.CreateFromReference(particleObj, entHandler, boundaryStrat, team, parentPos);
		}

		public override Component Create(JObject particleObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offsetPos, float mod =1)
		{
			string id = particleObj.Value<string>("id");
			List<Color> colors = Util.LoadColors(particleObj.Value<JArray>("colors"));
			float scale = particleObj.Value<float>("scale");
			JObject shapeObj = particleObj.Value<JObject>("shape");
			Shape shape = new ShapeFactory().Create(shapeObj, scale, offsetPos);
			float friction = particleObj.Value<float>("friction");
			MinMax<float> lifetimeRange = particleObj["lifetimeRange"].ToObject<MinMax<float>>();
			MinMax<float> velRange = particleObj["velRange"].ToObject<MinMax<float>>();
			MinMax<float> turnRateRange = particleObj["turnRateRange"].ToObject<MinMax<float>>();

			return new Particle(id, path, SwinGame.PointAt(0, 0), offsetPos, shape, colors,
				friction, lifetimeRange, velRange, turnRateRange, boundaryStrat, team);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// A ship engine for moving the ship around
	/// </summary>
	public class Engine : Component
	{
		public float Thrust { get; private set; }
		public float MaxVel { get; private set; }
		private float turnRate;

		private int mass;
		public override int Mass { get { return base.Mass + mass; } }
		private bool thrusting;

		public Engine(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int health, Vector vel, Vector dir, BoundaryStrategy boundaryStrat,
			Team team, List<Component> components, float thrust, float maxVel, float turnRate, int mass
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, boundaryStrat, team)
		{
			this.mass = mass <= 0 ? 1 : mass;
			Thrust = thrust;
			MaxVel = maxVel;
			this.turnRate = turnRate;
			childComponents = components;
		}

		/// <summary>
		/// Ask the engine to thrust
		/// </summary>
		/// <param name="vDir">Direction of thrust</param>
		/// <param name="vel">ship's current velocity</param>
		/// <param name="mass">ship's mass</param>
		/// <returns>velocity clamped to engine's max velocity</returns>
		public Vector ApplyForce(Vector vDir, Vector vel, int mass) {
			//apply force to the vector
			Vector modified = vel;
			Vector force = new Vector {
				X = vDir.X * (Thrust / mass),
				Y = vDir.Y * (Thrust / mass)
			};
			modified += force;

			if (modified.Magnitude > MaxVel)
				return vel;
			else {
				thrusting = true;
				return modified;
			}
		}

		public double Turn(float turnStrength) {
			return (turnRate * turnStrength);
		}

		public override void Update() {
			if (thrusting) {
				foreach(Emitter e in childComponents.OfType<Emitter>()) {
					e.Activate();
				}
				thrusting = false;
			}

			base.Update();
		}

		public override void TeleportTo(Point2D target) {
			base.TeleportTo(target);
			childComponents?.TeleportTo(target);
		}
	}

	/// <summary>
	/// Engine Factory
	/// </summary>
	public class EngineFactory : ComponentFactory
	{
		public override Component CreateFromReference(JObject engineObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod =1) {
			string path = SwinGame.AppPath() + engineObj.Value<string>("path");
			Point2D offsetPos = engineObj["pos"].ToObject<Point2D>().Multiply(10);

			return base.CreateFromReference(engineObj, entHandler, boundaryStrat, team, offsetPos);
		}

		public override Component Create(JObject engineObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offsetPos, float mod = 1)
		{
			string id = engineObj.Value<string>("id");
			float thrust = engineObj.Value<float>("thrust") * mod;
			float maxVel = engineObj.Value<float>("maxVel") * mod;
			float turnRate = engineObj.Value<float>("turnRate") * mod;
			int mass = engineObj.Value<int>("mass");
			float scale = engineObj.Value<float>("scale");
			JObject shapeObj = engineObj.Value<JObject>("shape");
			Shape shape = new ShapeFactory().Create(shapeObj, scale, offsetPos);

			JArray emitterObj = engineObj.Value<JArray>("emitters");
			List<Component> emitters = new EmitterFactory().CreateList(emitterObj, entHandler, boundaryStrat, team, offsetPos);

			return new Engine(id, path, SwinGame.PointAt(0, 0), offsetPos, shape,
				new List<Color> { Color.White }, 1, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1),
				boundaryStrat, team, emitters, thrust, maxVel, turnRate, mass);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Ship tool that fires ammo
	/// </summary>
	public class Tool : Component
	{
		private float cooldown;
		public Ammo Ammo { get { return childComponents.OfType<Ammo>().First(); } }
		private IHandlesEntities entHandler;

		private int mass;
		public override int Mass { get { return base.Mass + mass; } }

		private CooldownHandler cdHandler;

		public Tool(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int health, Vector vel, Vector dir, float cooldown, BoundaryStrategy boundaryStrat,
			Team team, List<Component> children, int mass, IHandlesEntities entHandler
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, vel, dir, boundaryStrat, team)
		{
			this.cooldown = cooldown;
			this.mass = mass <= 0 ? 1 : mass;
			childComponents = children;
			this.entHandler = entHandler;
			cdHandler = new CooldownHandler(cooldown*1000);
		}

		/// <summary>
		/// activate the tool and fire it's ammo
		/// </summary>
		public void Activate() {
			if (!cdHandler.IsOnCooldown()) {
				JObject ammoObj = Util.Deserialize(Ammo.FilePath);

				Ammo newAmmo = new AmmoFactory().Create(ammoObj, FilePath, entHandler, boundaryStrat, Team, SwinGame.PointAt(0,0)) as Ammo;
				newAmmo.TeleportTo(RealPos);

				newAmmo.Init(RealPos, Dir, Vel);
				entHandler.Track(newAmmo);
				cdHandler.StartCooldown();
			}
		}

		public override void Update() {
			base.Update();
			Ammo.Sleep();
		}

		public override void TeleportTo(Point2D target) {
			base.TeleportTo(target);
			childComponents?.TeleportTo(target);
		}
	}

	/// <summary>
	/// Tool factory
	/// </summary>
	public class ToolFactory : ComponentFactory
	{
		public override Component CreateFromReference(JObject toolObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod =1) {
			string path = SwinGame.AppPath() + toolObj.Value<string>("path");
			Point2D offsetPos = toolObj["pos"].ToObject<Point2D>().Multiply(10);
			offsetPos = offsetPos.Add(parentPos);

			return base.CreateFromReference(toolObj, entHandler, boundaryStrat, team, offsetPos);
		}

		public override Component Create(JObject toolObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offsetPos, float mod =1) 
		{
			string id = toolObj.Value<string>("id");
			int mass = toolObj.Value<int>("mass");
			float scale = toolObj.Value<float>("scale");
			float cooldown = toolObj.Value<float>("cooldown") / mod;
			JObject shapeObj = toolObj.Value<JObject>("shape");
			List<Color> colors = Util.LoadColors(toolObj.Value<JArray>("colors"));
			Shape shape = new ShapeFactory().Create(shapeObj, scale, offsetPos);

			JObject ammoObj = toolObj.Value<JObject>("ammo");
			Component ammo = new AmmoFactory().CreateFromReference(ammoObj, entHandler, boundaryStrat, team, offsetPos, mod);

			return new Tool(id, path, SwinGame.PointAt(0, 0), offsetPos, shape,
				colors, 1, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1), cooldown,
				boundaryStrat, team, new List<Component>() { ammo }, mass, entHandler);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TaskForceUltra.src.GameModule.AI.strategies;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Ammo object
	/// </summary>
	public class Ammo : Component, ICollides
	{
		private float lifetime;
		private int mass;
		protected float maxVel;
		protected float thrustForce;
		protected float turnRate;
		protected bool thrusting;

		public override int Mass { get { return base.Mass + mass; } }
		public int Damage { get; private set; }

		protected bool sleep;

		private CooldownHandler cdHandler;

		public Ammo(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int mass, int damage, float lifetime, float vel,
			float turnRate, BoundaryStrategy boundaryStrat, Team team
		) : base(id, filePath, refPos, offsetPos, shape, colors, 1, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1), boundaryStrat, team)
		{
			Damage = damage;
			this.lifetime = lifetime < 0 ? 0 : lifetime;
			this.mass = mass <= 0 ? 1 : mass;
			maxVel = vel;
			thrustForce = 0;
			this.turnRate = turnRate;
		}

		public override void Update() {
			if (sleep)
				return;

			base.Update();
			if (cdHandler != null) {
				if (cdHandler.IsOnCooldown()) {
					Thrust(Dir);
				}
				else Kill(Team.None);
			}
		}

		/// <summary>
		/// Accelerate the Ammo object
		/// </summary>
		/// <param name="vDir">vector along which to accelerate</param>
		public virtual void Thrust(Vector vDir) {
			thrusting = true;
			Vector deltaV = Dir.Multiply(thrustForce / mass);
			Vel = (Vel.AddVector(deltaV)).LimitToMagnitude(maxVel);
		}

		public override void Draw() {
			if (sleep)
				return;

			if (cdHandler != null)
				base.Draw();

			DebugDraw();
		}

		/// <summary>
		/// initialise an ammo object
		/// </summary>
		/// <param name="pos">spawning position</param>
		/// <param name="dir">spawning direction</param>
		/// <param name="vel">spawning velocity</param>
		public virtual void Init(Point2D pos, Vector dir, Vector vel) {
			TeleportTo(pos);
			theta = Dir.AngleTo(dir) * Math.PI / 180;

			maxVel += vel.Magnitude;
			thrustForce = maxVel;
			cdHandler = new CooldownHandler(lifetime * 1000);
			cdHandler.StartCooldown();
		}

		public void ReactToCollision(int dmg, Vector collidingVel, int collidingMass, Team collider, bool forceReaction = false) {
			Kill(Team.None);
		}

		/// <summary>
		/// Deactivate ammo
		/// </summary>
		public void Sleep() {
			sleep = true;
		}

		protected override void DebugDraw() {
			if (DebugMode.IsDebugging(Debugging.Ammo))
				Debug();
		}
	}

	/// <summary>
	/// Ammo Factory
	/// </summary>
	public class AmmoFactory : ComponentFactory
	{
		public override Component Create(JObject ammoObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod = 1) {
			string id = ammoObj.Value<string>("id");
			List<Color> colors = Util.LoadColors(ammoObj.Value<JArray>("colors"));
			int mass = ammoObj.Value<int>("mass");
			int damage = (int)(ammoObj.Value<int>("damage") * mod);
			float lifetime = ammoObj.Value<float>("lifetime") * mod;
			float vel = ammoObj.Value<float>("vel") * mod;
			float maxVel = ammoObj.Value<float>("maxVel") * mod;
			float turnRate = ammoObj.Value<float>("turnRate") * mod;
			float scale = ammoObj.Value<float>("scale");
			JObject shapeObj = ammoObj.Value<JObject>("shape");
			Shape shape = new ShapeFactory().Create(shapeObj, scale, parentPos);
			string behaviour = ammoObj.Value<string>("behaviour");

			if (team == Team.Computer)
				colors = new List<Color> { Color.Yellow };

			switch(behaviour) {
				case "seek":
					JArray emitterObj = ammoObj.Value<JArray>("emitters");
					List<Component> emitters = new EmitterFactory().CreateList(emitterObj, entHandler, boundaryStrat, team, parentPos, mod);
					SeekAmmo result = new SeekAmmo(id, path, SwinGame.PointAt(0, 0), parentPos, shape, colors, mass, damage, lifetime, vel, maxVel, turnRate, emitters, boundaryStrat, entHandler, team);
					result.AIStrat = new ChaseStrategy(result, entHandler, 0);
					return result;
				case "static":
					return new Ammo(id, path, SwinGame.PointAt(0, 0), parentPos, shape, colors, mass, damage, lifetime, vel, turnRate, boundaryStrat, team);
				default:
					return new Ammo(id, path, SwinGame.PointAt(0, 0), parentPos, shape, colors, mass, damage, lifetime, vel, turnRate, boundaryStrat, team);
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.AI;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Ammo that locks onto and chases objects on other teams
	/// </summary>
	public class SeekAmmo : Ammo, IAIEntity
	{
		public float MaxVel {
			get { return maxVel; }
			set { maxVel = value; }
		}
		private float accel;
		private IHandlesEntities entityHandler;
		private AIStrategy aiStrat;
		public AIStrategy AIStrat { set { aiStrat = value; } }
		private CooldownHandler primingTimer; //delay before we start seeking behaviour
		private List<Component> emitters;

		public SeekAmmo(string id, string filePath, Point2D refPos, Point2D offsetPos,
			Shape shape, List<Color> colors, int mass, int damage, float lifetime, float vel, float maxVel,
			float turnRate, List<Component> emitters, BoundaryStrategy boundaryStrat, IHandlesEntities entHandler, Team team
		) : base(id, filePath, refPos, offsetPos, shape, colors, mass, damage, lifetime, vel, turnRate, boundaryStrat, team)
		{
			entityHandler = entHandler;
			MaxVel = maxVel;
			primingTimer = new CooldownHandler(1000);
			primingTimer.StartCooldown();
			this.emitters = emitters;
			accel = vel;
		}

		public override void Update() {
			if (sleep)
				return;

			aiStrat.Update();
			HandleEmitters();
			base.Update();
		}

		private void HandleEmitters() {
			if (emitters == null)
				return;

			if (thrusting && !primingTimer.IsOnCooldown()) {
				foreach (Emitter e in emitters) {
					e.Update();
					e.Activate();
					e.TeleportTo(RealPos);
				}
				thrusting = false;
			}
		}

		/// <summary>
		/// initialise the seeking ammo
		/// </summary>
		/// <param name="pos">spawning position</param>
		/// <param name="dir">spawning direction</param>
		/// <param name="vel">spawning velocity</param>
		public override void Init(Point2D pos, Vector dir, Vector vel) {
			emitters.TeleportTo(pos);
			base.Init(pos, dir, vel);
			thrustForce = accel;
		}

		public void TurnTo(Vector targetDir, float turnStrength = 1) {
			if (primingTimer.IsOnCooldown())
				return;

			double desiredTheta = Dir.AngleTo(targetDir) * Math.PI / 180;

			theta += turnRate * turnStrength * Math.PI / 180;
			theta *= desiredTheta.GetSign();

			if (theta <= 0) {
				theta = theta.Clamp(desiredTheta, 0);
			}
			else {
				theta = theta.Clamp(0, desiredTheta);
			}
		}

		public void Fire() { }
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// base class for entity behaviour when it goes outside of the play area boundaries
	/// </summary>
	public abstract class BoundaryStrategy
	{
		protected Rectangle playArea;

		public BoundaryStrategy(Rectangle playArea) {
			this.playArea = playArea;
		}

		/// <summary>
		/// checks whether entity is in play area or not
		/// </summary>
		/// <param name="entity">entity to check</param>
		/// <returns>true or false</returns>
		protected bool IsInPlay(Entity entity) {
			if (entity != null)
				return entity.RealPos.InRect(playArea);
			else return false;
		}

		public abstract void Run(Entity e);
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// The entity will wrap around to the other side if it goes outside of the play area
	/// </summary>
	public class WrapBoundaryBehaviour : BoundaryStrategy
	{
		public WrapBoundaryBehaviour(Rectangle playArea) : base(playArea) { }

		public override void Run(Entity entity) {
			int padding = 10;

			if (!IsInPlay(entity)) {
				Point2D target = entity.RealPos;

				if (entity.RealPos.X < playArea.Left)
					target.X = playArea.Right - padding;
				else if (entity.RealPos.X > playArea.Right)
					target.X = playArea.Left + padding;

				if (entity.RealPos.Y < playArea.Top)
					target.Y = playArea.Bottom - padding;
				else if (entity.RealPos.Y > playArea.Bottom)
					target.Y = playArea.Top + padding;

				entity.TeleportTo(target);
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TaskForceUltra.src.GameModule.AI;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Random Dummy Asteroid
	/// </summary>
	public class Asteroid : Mover, ICollides, IAIEntity
	{
		public int Damage { get; private set; }
		private AIStrategy aiStrat;
		public AIStrategy AIStrat { set { aiStrat = value; } }
		private float turnRate;
		public float MaxVel { get; private set; }
		private int mass;
		public override int Mass { get { return (base.Mass + mass); } }

		public Asteroid(string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int mass, int health, float thrustForce, Vector dir, float turnRate, BoundaryStrategy boundaryStrat,
			Team team, int dmg, bool optimiseMe = false)
		: base(id, filePath, refPos, offsetPos, shape, colors, health, SwinGame.VectorFromAngle(dir.Angle, thrustForce), dir, boundaryStrat, team, optimiseMe)
		{
			this.turnRate = turnRate;
			this.mass = mass;
			Vel = dir.Multiply(thrustForce);
			Damage = dmg;
		}

		public override void Update() {
			theta += turnRate * Math.PI / 180;
			base.Update();
		}

		public override void Draw() {
			base.Draw();

			DrawDebug();
		}

		private void DrawDebug() {
			if (DebugMode.IsDebugging(Debugging.Ship))
				Debug();
		}

		public void ReactToCollision(int dmg, Vector collidingVel, int collidingMass, Team collidingTeam, bool forceReaction = false) {
			health -= dmg;

			float velTransferMod = ((float)collidingMass / (float)Mass);
			Vel = Vel.AddVector(collidingVel.Multiply(velTransferMod));
			Vel = Vel.LimitToMagnitude(MaxVel);

			if (health <= 0)
				Kill(collidingTeam);
		}

		public void TurnTo(Vector targetDir, float turnStrength = 1) { }

		public void Thrust(Vector vDir) { }

		public void Fire() { }
	}

	/// <summary>
	/// Asteroid Factory
	/// </summary>
	public class AsteroidFactory
	{
		public Asteroid Create(string filePath, Rectangle playArea) {
			//try
			JObject obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath));
			string id = obj.Value<string>("id");
			List<Color> colors = Util.LoadColors(obj.Value<JArray>("colors"));
			int health = obj.Value<int>("baseHealth");
			int damage = obj.Value<int>("damage");
			int mass = obj.Value<int>("mass");

			JToken sizeObj = obj.GetValue("sizeRange");
			JToken edgesObj = obj.GetValue("edgesRange");
			JToken turnRateRangeObj = obj.GetValue("turnRateRange");
			JToken velRangeObj = obj.GetValue("velRange");
			MinMax<float> sizeRange = new MinMax<float>(sizeObj.Value<float>("Min"), sizeObj.Value<float>("Max"));
			MinMax<float> edgesRange = new MinMax<float>(edgesObj.Value<float>("Min"), edgesObj.Value<float>("Max"));
			MinMax<float> turnRateRange = new MinMax<float>(turnRateRangeObj.Value<float>("Min"), turnRateRangeObj.Value<float>("Max"));
			MinMax<float> velRange = new MinMax<float>(velRangeObj.Value<float>("Min"), velRangeObj.Value<float>("Max"));

			Vector dir = Util.RandomUnitVector();
			float vel = Util.RandomInRange(velRange);
			float turnRate = Util.RandomInRange(turnRateRange);
			BoundaryStrategy boundaryStrat = new WrapBoundaryBehaviour(playArea);

			float size = Util.RandomInRange(sizeRange);
			mass *= (int)size/10;
			Shape shape = new ShapeFactory().CreateCircleApprox(size, (int)Util.RandomInRange(edgesRange));
			Point2D spawnPos = Util.RandomPointInRect(playArea);

			Asteroid result = new Asteroid(id, filePath, SwinGame.PointAt(0, 0), SwinGame.PointAt(-size, size), shape, colors, mass, health, vel, dir, turnRate, boundaryStrat, Team.Computer, damage);
			result.TeleportTo(spawnPos);

			result.AIStrat = new AIStrategyFactory(0, 0).Create(result);
			return result;
		}
	}
}

﻿using System;
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra
{
	public enum SelectionType { Ship, Difficulty, Level }
	public enum EnvMod { Nebula, Flare, Blackhole, Void, Radioactive }
	public enum DifficultyType { Easy, Medium, Hard }
	public enum ShipAction { Forward, Backward, StrafeLeft, StrafeRight, TurnLeft, TurnRight, Shoot, ActivatePowerup }
	public enum ControllerType { Player1, Player2, Player3, Player4, Computer }
	public enum Team { Team1, Team2, Team3, Team4, Computer, None }
	public enum GameResultType { Time, Points, Result }
	public enum BattleResult { Loss = 0, Win = 1 }
	public enum Debugging { Ship, Camera, Ammo, Particle, Component, Nodes }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaskForceUltra.src.GameModule.AI;

namespace TaskForceUltra
{
	/// <summary>
	/// Difficulty object
	/// </summary>
	public struct Difficulty
	{
		public string Id;
		public float SpawnTimer;
		public float AIMod;
		public int DifficultyLevel;
		public int ShootCooldown;

		public Difficulty(string id, float spawnTimer, float aiMod, int diffLevel, int shootCooldown) {
			Id = id;
			SpawnTimer = spawnTimer;
			AIMod = aiMod;
			DifficultyLevel = diffLevel;
			ShootCooldown = shootCooldown;
		}
	}

	/// <summary>
	/// static accessor for Difficulty settings. Might as well try having one for fun
	/// </summary>
	//prevent inheritance
	public static class DifficultySetting
	{
		private static string dirPath = SwinGame.AppPath() + "\\resources\\data\\difficulty";

		public static Difficulty Fetch(string id) {
			switch (id.ToLower()) {
				case "easy":
					return Easy;
				case "medium":
					return Medium;
				case "hard":
					return Hard;
				default: return Easy;
			}
		}

		//difficulty properties
		public static Difficulty Easy {
			get { return ReadDifficulty(dirPath + "\\easy.json"); } }
		public static Difficulty Medium {
			get { return ReadDifficulty(dirPath + "\\medium.json"); } }
		public static Difficulty Hard {
			get { return ReadDifficulty(dirPath + "\\hard.json"); } }

		private static Difficulty ReadDifficulty(string filePath) {
			JObject diffObj = Util.Deserialize(filePath);

			string id = diffObj.Value<string>("difficultyId");
			float spawnTimer = diffObj.Value<float>("spawnTimer");
			float aiMod = diffObj.Value<float>("aiMod");
			int diffLevel = diffObj.Value<int>("difficultyLevel");
			int shootCooldown = diffObj.Value<int>("shootCooldown");

			return new Difficulty(id, spawnTimer, aiMod, diffLevel, shootCooldown);
		}
	}
}

﻿namespace TaskForceUltra
{
	/// <summary>
	/// struct that defines width and height
	/// </summary>
	public struct Size2D<T>
	{
		public T W { get; set; }
		public T H { get; set; }

		public Size2D(T w, T h) {
			W = w;
			H = h;
		}
	}
}

﻿namespace TaskForceUltra
{
	/// <summary>
	/// Struct that defines min and max values
	/// </summary>
	public struct MinMax<T>
	{
		public T Min { get; set; }
		public T Max { get; set; }

		public MinMax(T min, T max) {
			Min = min;
			Max = max;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra
{
	/// <summary>
	/// handles debugging state
	/// </summary>
	public static class DebugMode
	{
		private static bool ship = false;
		private static bool ammo = false;
		private static bool particle = false;
		private static bool level = false;
		private static bool component = false;
		private static bool nodes = false;

		/// <summary>
		/// checks whether the type is being debugged
		/// </summary>
		/// <param name="type">debugging enum</param>
		/// <returns></returns>
		public static bool IsDebugging(Debugging type) {
			switch (type) {
				case Debugging.Ship:
					return ship;
				case Debugging.Ammo:
					return ammo;
				case Debugging.Particle:
					return particle;
				case Debugging.Camera:
					return level;
				case Debugging.Component:
					return component;
				case Debugging.Nodes:
					return nodes;
				default:
					return false;
			}
		}

		/// <summary>
		/// Toggle debugging states
		/// </summary>
		/// <param name="toggles">to toggle</param>
		private static void ToggleDebugState(Debugging[] toggles) {
			foreach(Debugging toggle in toggles) {
				switch (toggle) {
					case Debugging.Ammo:
						ammo = !ammo;
						break;
					case Debugging.Component:
						component = !component;
						break;
					case Debugging.Camera:
						level = !level;
						break;
					case Debugging.Particle:
						particle = !particle;
						break;
					case Debugging.Ship:
						ship = !ship;
						break;
					case Debugging.Nodes:
						nodes = !nodes;
						break;
				}
			}
		}

		/// <summary>
		/// Listen for player input and update debugging state
		/// </summary>
		public static void HandleInput() {
			if (SwinGame.KeyDown(KeyCode.CtrlKey)) {
				//debug all
				if (typed(KeyCode.DKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Ammo, Debugging.Component, Debugging.Camera, Debugging.Particle, Debugging.Ship, Debugging.Nodes });
				}
				//debug ship
				else if (typed(KeyCode.SKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Component, Debugging.Ship });
				}
				//debug ammo
				else if (typed(KeyCode.AKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Ammo });
				}
				//debug component
				else if (typed(KeyCode.CKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Component });
				}
				//debug level
				else if (typed(KeyCode.LKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Camera });
				}
				//debug particle
				else if (typed(KeyCode.PKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Particle });
				}
			}
		}

		private static bool typed(KeyCode key) {
			return SwinGame.KeyTyped(key);
		}

	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra
{
	/// <summary>
	/// logging stuff
	/// </summary>
	public static class Log
	{
		/// <summary>
		/// Log point to console
		/// </summary>
		/// <param name="pos">point</param>
		public static void Pos(Point2D pos) {
			Console.WriteLine($"x: {pos.X} + y: {pos.Y}");
		}

		/// <summary>
		/// Log point to console with label
		/// </summary>
		/// <param name="id">point name</param>
		/// <param name="pos">point</param>
		public static void Pos(string id, Point2D pos) {
			Console.Write($"id: {id} ");
			Pos(pos);
		}

		/// <summary>
		/// Log vector to console
		/// </summary>
		/// <param name="vec">Vector</param>
		public static void Vec(Vector vec) {
			Console.WriteLine($"x: {vec.X} y: {vec.Y}");
		}

		/// <summary>
		/// Log vector to console with label
		/// </summary>
		/// <param name="id">Vector name</param>
		/// <param name="vec">vector</param>
		public static void Vec(string id, Vector vec) {
			Console.Write($"id: {id} ");
			Vec(vec);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra
{
	/// <summary>
	/// Extension methods for movements
	/// </summary>
	public static class MoveExtensions
	{
		/// <summary>
		/// Move a point
		/// </summary>
		/// <param name="p">point</param>
		/// <param name="vel">velocity</param>
		/// <returns>Moved Point</returns>
		public static Point2D Move(this Point2D p, Vector vel) {
			Point2D result = p;
			return result += vel;
		}

		/// <summary>
		/// Move a line segment
		/// </summary>
		/// <param name="l">linesegment</param>
		/// <param name="vel">velocity</param>
		/// <returns>Moved Line segemtn</returns>
		public static LineSegment Move(this LineSegment l, Vector vel) {
			LineSegment result = l;
			result.StartPoint = result.StartPoint.Move(vel);
			result.EndPoint = result.EndPoint.Move(vel);

			return result;
		}

		/// <summary>
		/// Move a bunch of line segments
		/// </summary>
		/// <param name="s">list of line segments</param>
		/// <param name="vel">velocity</param>
		/// <returns>list of line segments</returns>
		public static List<LineSegment> Move(this List<LineSegment> s, Vector vel) {
			List<LineSegment> result = s;
			for (int i = 0; i < result.Count; ++i) {
				result[i] = result[i].Move(vel);
			}

			return result;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra
{
	/// <summary>
	/// Extension methods for rotating stuff
	/// </summary>
	public static class RotateExtensions
	{
		/// <summary>
		/// Rotate Vector
		/// </summary>
		/// <param name="v">Vector</param>
		/// <param name="theta">radians</param>
		public static Vector Rotate(this Vector v, double theta) {
			Vector result = SwinGame.VectorTo(0, 0);
			result.X = (float)(v.X * Math.Cos(theta) - v.Y * Math.Sin(theta));
			result.Y = (float)(v.Y * Math.Cos(theta) + v.X * Math.Sin(theta));

			return result;
		}

		/// <summary>
		/// Rotate a point around an anchor of rotation
		/// </summary>
		/// <param name="p">point</param>
		/// <param name="pos">anchor of rotation</param>
		/// <param name="theta">radians</param>
		public static Point2D Rotate(this Point2D p, Point2D pos, double theta) {
			Point2D temp = new Point2D {
				X = p.X - pos.X,
				Y = p.Y - pos.Y
			};

			Point2D result = new Point2D {
				X = (float)(temp.X * Math.Cos(theta) - temp.Y * Math.Sin(theta) + pos.X),
				Y = (float)(temp.Y * Math.Cos(theta) + temp.X * Math.Sin(theta) + pos.Y)
			};

			return result;
		}

		/// <summary>
		/// Rotate Linesegment around an anchor of rotation
		/// </summary>
		/// <param name="l">linesegment</param>
		/// <param name="pos">anchor of rotation</param>
		/// <param name="theta">radians</param>
		public static LineSegment Rotate(this LineSegment l, Point2D pos, double theta) {
			LineSegment result = l;
			result.StartPoint = result.StartPoint.Rotate(pos, theta);
			result.EndPoint = result.EndPoint.Rotate(pos, theta);

			return result;
		}

		/// <summary>
		/// Rotate List of line segments
		/// </summary>
		/// <param name="s">list of linesegments</param>
		/// <param name="pos">anchor of rotation</param>
		/// <param name="theta">radians</param>
		public static List<LineSegment> Rotate(this List<LineSegment> s, Point2D pos, double theta) {
			if (s == null) return null;

			List<LineSegment> result = s;
			for (int i = 0; i < result.Count; ++i) {
				result[i] = result[i].Rotate(pos, theta);
			}

			return result;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;

namespace TaskForceUltra
{
	/// <summary>
	/// Utility extension methods
	/// </summary>
	public static class UtilExtensions
	{
		/// <summary>
		/// Clamp float between values
		/// </summary>
		public static float Clamp(this float x, float min, float max) {
			if (x < min)
				return min;
			else if (x > max)
				return max;
			return x;
		}

		/// <summary>
		/// Clamp double between values
		/// </summary>
		public static double Clamp(this double x, double min, double max) {
			if (x < min)
				return min;
			else if (x > max)
				return max;
			return x;
		}

		/// <summary>
		/// clamp int between values
		/// </summary>
		public static int Clamp(this int x, int min, int max) {
			if (x < min)
				return min;
			else if (x > max)
				return max;
			return x;
		}

		/// <summary>
		/// get the sign of a float
		/// </summary>
		public static float GetSign(this float x) {
			if (x < 0)
				return -1;
			if (x > 0)
				return 1;
			return 0;
		}

		/// <summary>
		/// get the sign of a double
		/// </summary>
		public static double GetSign(this double x) {
			if (x < 0)
				return -1;
			if (x > 0)
				return 1;
			return 0;
		}

		/// <summary>
		/// Multiply point by float
		/// </summary>
		public static Point2D Multiply(this Point2D p, float x) {
			return SwinGame.PointAt(p.X * x, p.Y * x);
		}

		/// <summary>
		/// Subtract point from another point
		/// </summary>
		public static Point2D Subtract(this Point2D p1, Point2D p2) {
			return SwinGame.PointAt(p1.X - p2.X, p1.Y - p2.Y);
		}

		/// <summary>
		/// Multiply point by an int
		/// </summary>
		public static LineSegment Multiply(this LineSegment l, int x) {
			return SwinGame.CreateLine(l.StartPoint.Multiply(x), l.EndPoint.Multiply(x));
		}

		//////////////////////
		// drawing linesegment
		//////////////////////
		public static void Draw(this LineSegment l, Color clr, Point2D offset) {
			LineSegment result = SwinGame.CreateLine(l.StartPoint.Add(offset), l.EndPoint.Add(offset));
			SwinGame.DrawLine(clr, result);
		}

		//////////////////////////////////
		//component list extension methods
		//////////////////////////////////
		public static void TeleportTo(this List<Component> componentList, Point2D target) {
			foreach (Component c in componentList) {
				c?.TeleportTo(target);
			}
		}

		public static int Mass(this List<Component> componentList) {
			int result = 0;
			foreach(Component c in componentList) {
				if (c != null)
					result += c.Mass;
			}
			return result;
		}

		public static void Update(this List<Component> componentList) {
			if (componentList == null)
				return;

			foreach (Component c in componentList) {
				c?.Update();
			}
		}

		public static void Draw(this List<Component> componentList) {
			if (componentList == null)
				return;

			foreach (Component c in componentList) {
				c?.Draw();
			}
		}

		public static void SetVel(this List<Component> componentList, Vector v) {
			if (componentList == null)
				return;

			foreach (Component c in componentList) {
				c?.SetVel(v);
			}
		}

		public static void Turn(this List<Component> componentList, double theta) {
			if (componentList == null)
				return;

			foreach (Component c in componentList) {
				c?.Turn(theta);
			}
		}
	}
}

﻿using System;
using System.Collections.Generic;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra
{
	/// <summary>
	/// Utility helper functions
	/// </summary>
	public static class Util
	{
		//////////////////////////
		// Random number generation
		/////////////////////////
		//shorten the call for random number
		public static int Rand(int min, int max) {
			Random rng = new Random(Guid.NewGuid().GetHashCode());
			return rng.Next(min, max);
		}
		public static int Rand(int max) {
			Random rng = new Random(Guid.NewGuid().GetHashCode());
			return rng.Next(max);
		}
		public static int Rand() {
			Random rng = new Random(Guid.NewGuid().GetHashCode());
			return rng.Next();
		}

		//random return values
		public static float RandomInRange(MinMax<float> x) {
			float result = Rand((int)(x.Min * 1000), (int)(x.Max * 1000));
			result /= 1000f;
			return result;
		}

		public static Point2D RandomPointInRect(Rectangle rect) {
			int x = Rand((int)rect.X, (int)rect.Right);
			int y = Rand((int)rect.Y, (int)rect.Bottom);

			return SwinGame.PointAt(x, y);
		}

		public static Vector RandomUnitVector() {
			float x = (Rand(2000) - 1000);
			x /= 1000;
			float y = (Rand(2000) - 1000);
			y /= 1000;
			return SwinGame.VectorTo(x, y).UnitVector;
		}

		///////////////////////////////
		// Json deserialisation helpers
		//////////////////////////////
		public static JObject Deserialize(string filePath) {
			try {
				string buffer = File.ReadAllText(filePath);
				JObject obj = JsonConvert.DeserializeObject<JObject>(buffer);
				return obj;
			}
			catch (Exception e) {
				Console.WriteLine($"error deserializing from {filePath}");
				Console.WriteLine(e);
				return null;
			}
		}

		public static Color GetRGBColor(JToken color) {
			return SwinGame.RGBColor(color.Value<byte>("R"), color.Value<byte>("G"), color.Value<byte>("B"));
		}

		public static List<Color> LoadColors(JArray colorsObj) {
			List<Color> result = new List<Color>();

			foreach (JObject c in colorsObj) {
				result.Add(GetRGBColor(c));
			}
			return result;
		}

		public static Color DeserializeKeyedColor(JArray colorObj, string key) {
			foreach (JObject c in colorObj) {
				if (c.ContainsKey(key))
					return GetRGBColor(c.GetValue(key));
			}
			return Color.White;
		}
	}
}
