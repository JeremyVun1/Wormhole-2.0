using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class MenuCommandFactory
	{
		MenuModule menuModule;

		public MenuCommandFactory(MenuModule m)
		{
			menuModule = m;
		}

		public ICommand Create(string action, string payload)
		{
			switch(action.ToLower())
			{
				case "navto" :
					return new NavToCommand(menuModule, payload);
				case "exit":
					return new ExitCommand(menuModule);
				case "save":
					return new SaveCommand(menuModule);
				case "load":
					return new LoadCommand(menuModule);
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
