using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to select a level to play on
	/// </summary>
	public class SelectLevelCommand : ICommand
	{
		private IMenuModule menuModule;
		private string id;

		public SelectLevelCommand(IMenuModule m, string id) {
			menuModule = m;
			this.id = id;
		}

		public void Execute() {
			menuModule.AddSelection(SelectionType.Level, id);
		}

		public void Undo() {
			menuModule.RemoveSelection(SelectionType.Level);
		}
	}
}
