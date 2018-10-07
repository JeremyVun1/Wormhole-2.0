using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	public class SelectShipCommand : ICommand
	{
		private IMenuModule menuModule;
		private string id;

		public SelectShipCommand(IMenuModule m, string id) {
			menuModule = m;
			this.id = id;
		}

		public void Execute() {
			menuModule.AddSelection(SelectionType.Ship, id);
		}
	}
}
