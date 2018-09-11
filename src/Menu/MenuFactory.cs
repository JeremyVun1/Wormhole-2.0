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

		public MenuFactory(string dir)
		{
			dirPath = dir;
		}

		private void ReadMenuData(string file, MenuModule menus)
		{
			string buffer = File.ReadAllText(dirPath + file);
			Menu result = JsonConvert.DeserializeObject<Menu>(buffer);
			menus.AddMenu(result.MenuID, result);
		}

		public MenuModule Create(Player player, ShipList shipList, LevelList levelList)
		{
			MenuModule result = new MenuModule(player, shipList, levelList);

			//IShipList shipList, ILevelList levelList, int money, IDiffList diff

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

			return result;
		}
	}
}
