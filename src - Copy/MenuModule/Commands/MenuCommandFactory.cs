using System;
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

		public ICommand Create(string action, string id, string parentId) {
			switch (action.ToLower()) {
				case "navto":
					return new NavToCommand(menuModule, id, parentId);
				case "increasevolume":
					return new IncreaseVolumeCommand();
				case "decreasevolume":
					return new DecreaseVolumeCommand();
				case "exit":
					return new ExitMenuCommand(menuModule);
				case "selectship":
					return new SelectShipCommand(menuModule, id);
				case "selectlevel":
					return new SelectLevelCommand(menuModule, id);
				case "selectdifficulty":
					return new SelectDifficultyCommand(menuModule, id);
				case "play":
					return new PlayCommand(menuModule);
				default:
					return null;
			}
		}
	}
}
