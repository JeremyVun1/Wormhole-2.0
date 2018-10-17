using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	public class SelectionGroup
	{
		public SelectionType selectionGroup { get; private set; }
		private List<Button> childButtons;

		public SelectionGroup(SelectionType type) {
			selectionGroup = type;
			childButtons = new List<Button>();
		}

		public void Add(Button b) {
			if (b!=null)
				childButtons.Add(b);
		}
		
		public void Remove(Button b) {
			if (b != null && childButtons.Contains(b))
				childButtons.Remove(b);
		}

		public void Update() {
			foreach(Button b in childButtons) {
				b.Update();
			}
		}

		public void Draw() {
			foreach(Button b in childButtons) {
				b.Draw();
			}
		}

		public void ResetButtons() {
			foreach(Button b in childButtons) {
				b.Reset();
			}
		}
	}
}
