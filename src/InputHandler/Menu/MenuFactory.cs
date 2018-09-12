using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class MenuFactory
	{
		private readonly string dirPath;
		private CommandButtonFactory buttonFac;
		private TextBoxFactory txtBoxFac;

		public MenuFactory(string dir)
		{
			dirPath = dir;
		}

		private void ReadMenuData(string file, MenuModule menus)
		{
			string buffer = File.ReadAllText(dirPath + file);
			dynamic menuObj = JsonConvert.DeserializeObject(buffer);

			//create buttons and textboxes to inject into menu
			List<IMenuElement> btns = buttonFac.CreateList(menuObj.Buttons, menuObj.ElementColors);
			List<IMenuElement> txtboxes = txtBoxFac.CreateList(menuObj.TextBoxes, menuObj.ElementColors);
			Menu result = new Menu(buffer, btns, txtboxes);

			menus.AddMenu(result.MenuId, result);
		}

		public MenuModule Create(Player player, ShipList shipList, LevelList levelList)
		{
			MenuModule result = new MenuModule(player, shipList, levelList);

			buttonFac = new CommandButtonFactory(result);
			txtBoxFac = new TextBoxFactory();

			//help
			ReadMenuData("\\help.json", result);
			//highscores
			ReadMenuData("\\highscores.json", result);
			//main
			ReadMenuData("\\main.json", result);
			//options
			ReadMenuData("\\options.json", result);
			//score screen
			ReadMenuData("\\scorescreen.json", result);
			//select
			ReadMenuData("\\select.json", result);

			result.ChangeMenu("Main");

			return result;
		}
	}
}
