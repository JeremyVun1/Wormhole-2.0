using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	public class MenuCommandFactory
	{
		IMenuModule menuModule;

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
