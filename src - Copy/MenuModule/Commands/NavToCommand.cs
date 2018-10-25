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
		private string parentId;

		public NavToCommand(IMenuModule m, string id, string parentId) {
			menuModule = m;
			this.id = id;
			this.parentId = parentId;
		}

		public void Execute() {
			menuModule.ChangeMenu(id);
		}

		public void Undo() {
			menuModule.ChangeMenu(parentId);
		}
	}
}
