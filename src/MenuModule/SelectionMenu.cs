using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	public class SelectionMenu : Menu
	{
		private SelectionGroup selectShipButtons;
		private SelectionGroup selectDifficultyButtons;
		private SelectionGroup selectLevelButtons;

		public SelectionMenu(string id, string title, List<MenuElement> elements,
			SelectionGroup ships, SelectionGroup diff, SelectionGroup levels
		) : base(id, title, elements) 
		{
			selectShipButtons = ships;
			selectDifficultyButtons = diff;
			selectLevelButtons = levels;
		}

		public override void Update() {
			base.Update();
			selectShipButtons.Update();
			selectDifficultyButtons.Update();
			selectLevelButtons.Update();
		}

		public override void Draw() {
			base.Draw();
			selectShipButtons.Draw();
			selectDifficultyButtons.Draw();
			selectLevelButtons.Draw();
		}
	}
}
