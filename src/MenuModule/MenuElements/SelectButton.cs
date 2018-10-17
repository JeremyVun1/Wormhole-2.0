using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.MenuModule
{
	public class SelectButton : Button
	{
		public SelectionType groupType { get; private set; }
		private SelectionGroup parent;

		public SelectButton(string id, ICommand command, Rectangle bounds, Color hover,
			Color fill, Color border, Color font, string text, SelectionGroup parent, SelectionType selectionType
		) : base(id, command, bounds, hover, fill, border, font, text)
		{
			groupType = selectionType;
			this.parent = parent;
		}

		public override void Click() {
			parent.ResetButtons();
			base.Click();
		}
	}
}
