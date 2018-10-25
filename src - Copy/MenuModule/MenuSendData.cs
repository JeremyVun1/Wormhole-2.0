using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Selected Game options for sending to the game controller to start a game
	/// </summary>
	public class MenuSendData
	{
		private IReceiveMenuData receiver;
		private Dictionary<SelectionType, string> selections;

		public MenuSendData(IReceiveMenuData receiver) {
			selections = new Dictionary<SelectionType, string>();
			this.receiver = receiver;
		}

		public void Send() {
			//check that all possible selection types are selected
			if (selections.Count() == Enum.GetNames(typeof(SelectionType)).Length)
				receiver.ReceiveMenuData(selections);
		}

		public void Add(SelectionType selection, string id) {
			if (selections.ContainsKey(selection))
				Remove(selection);
			selections.Add(selection, id);
		}

		public void Remove(SelectionType selection) {
			selections.Remove(selection);
		}
	}
}
