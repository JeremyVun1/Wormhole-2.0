using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Selection group for a set of select buttons. Can only pick one select button within the group
	/// </summary>
	public class SelectionGroup
	{
		public SelectionType selectionGroup { get; private set; }
		private List<Button> childButtons;

		public SelectionGroup(SelectionType type) {
			selectionGroup = type;
			childButtons = new List<Button>();
		}

		/// <summary>
		/// Add buttons to the selection group
		/// </summary>
		/// <param name="b">Button to add</param>
		public void Add(Button b) {
			if (b!=null)
				childButtons.Add(b);
		}
		
		/// <summary>
		/// Remove button from selection group
		/// </summary>
		/// <param name="b">Button to remove</param>
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
