using System;
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

			List<MenuElement> elements = menuElementFac.Create(textBoxesObj, buttonsObj, colorsObj, menuModule, id);

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

			List<MenuElement> elements = menuElementFac.Create(textBoxesObj, buttonsObj, colorsObj, menuModule, id);

			return new HighscoreMenu(id, title, elements);
		}

		public Menu CreateSelectMenu(string fileName, Dictionary<string, Shape> shipList, Dictionary<string, Level> levelList) {
			JObject menuObj = Util.Deserialize(dirPath + fileName);
			string id = menuObj.Value<string>("id").ToLower();
			string title = menuObj.Value<string>("title");
			JArray textBoxesObj = menuObj.Value<JArray>("textBoxes");
			JArray buttonsObj = menuObj.Value<JArray>("buttons");
			JArray colorsObj = menuObj.Value<JArray>("elementColors");
			List<MenuElement> elements = menuElementFac.Create(textBoxesObj, buttonsObj, colorsObj, menuModule, id);

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
			int count = 0;
			for (int i = 0; i < n; ++i) {
				string levelId = levelList.ElementAt(i).Key;

				//only allow level to be selected if it has been set to playable
				if (levelList.ElementAt(i).Value.Playable) {
					levelSelection.Add(menuElementFac.CreateSelectButton(id, levelId, levelSelectionBounds[count], SelectionType.Level, levelId, levelSelection, menuModule));
					count++;
				}
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
