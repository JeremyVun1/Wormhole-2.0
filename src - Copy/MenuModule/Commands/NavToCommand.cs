using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to navigate to another menu
	/// </summary>
	public class NavToCommand : ICommand
	{
		private IMenuModule menuModule;
		private string id;

		public NavToCommand(IMenuModule m, string id) {
			menuModule = m;
			this.id = id;
		}

		public void Execute() {
			menuModule.ChangeMenu(id);
		}
	}
}
