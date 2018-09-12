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
			switch((BtnAction)Enum.Parse(typeof(BtnAction), action))
			{
				case BtnAction.NavTo:
					return new NavToCommand(menuModule, payload);
				case BtnAction.Exit:
					return null; //new ExitCommand();
				case BtnAction.Save:
					return null; //new SaveCommand();
			}

			return null;
		}
	}
}
