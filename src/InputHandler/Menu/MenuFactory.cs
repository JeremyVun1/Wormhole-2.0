using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Wormhole
{
	public class MenuFactory
	{
		private readonly string dirPath;
		private ButtonFactory buttonFac;
		private TextBoxFactory txtBoxFac;

		public MenuFactory(string dir)
		{
			dirPath = dir;
		}

		private void ReadMenuData(string file, MenuModule menus)
		{
			string buffer = File.ReadAllText(dirPath + file);
			dynamic menuObj = JsonConvert.DeserializeObject(buffer);
			Menu result = new Menu(menuJObj, buttonFac.CreateList(menuObj.Buttons), txtBoxFac.CreateList(menuObj));


			//Menu result = JsonConvert.DeserializeObject<Menu>(buffer);
			Console.WriteLine(buffer);
			Console.WriteLine(result.Buttons.Count);

			menus.AddMenu(result.MenuId, result);
		}

		public MenuModule Create(Player player, ShipList shipList, LevelList levelList)
		{
			MenuModule result = new MenuModule(player, shipList, levelList);

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
