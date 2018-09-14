using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class NavToCommand : ICommand {

		private MenuModule menuModule;
		private string menuId;

		public NavToCommand(MenuModule m, string id)
		{
			menuModule = m;
			menuId = id;
		}

		public void Execute()
		{
			menuModule.ChangeMenu(menuId);
		}
	}
}
