using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wormhole.Delegates;

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

		private void ReadMenuData(string file, MenuModule menus, Level scene)
		{
			try
			{
				string buffer = File.ReadAllText(dirPath + file);
				dynamic menuObj = JsonConvert.DeserializeObject(buffer);

				//create buttons and textboxes to inject into menu
				List<IMenuElement> btns = buttonFac.CreateList(menuObj.Buttons, menuObj.ElementColors);
				List<IMenuElement> txtboxes = txtBoxFac.CreateList(menuObj.TextBoxes, menuObj.ElementColors);
				Menu result = new Menu(buffer, btns, txtboxes, scene);

				menus.AddMenu(result.MenuId, result);
			} catch (Exception e)
			{
				Log.Ex(e, String.Format("Error reading menu data from {0}", file));
			}
		}

		public MenuModule Create(Player player, ShipList shipList, LevelList levelList, ExitGame Exit)
		{
			MenuModule result = new MenuModule(player, shipList, levelList, Exit);

			buttonFac = new CommandButtonFactory(result);
			txtBoxFac = new TextBoxFactory();

			//help
			ReadMenuData("\\help.json", result, levelList.Fetch("MenuScene"));
			//highscores
			ReadMenuData("\\highscores.json", result, levelList.Fetch("MenuScene"));
			//main
			ReadMenuData("\\main.json", result, levelList.Fetch("MenuScene"));
			//options
			ReadMenuData("\\options.json", result, levelList.Fetch("MenuScene"));
			//score screen
			ReadMenuData("\\scorescreen.json", result, levelList.Fetch("MenuScene"));
			//select
			ReadMenuData("\\select.json", result, levelList.Fetch("MenuScene"));

			result.ChangeMenu("Main");

			return result;
		}
	}
}
