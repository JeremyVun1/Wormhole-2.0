using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public interface IControllableMenu
	{
		void ChangeMenu(string id);
		void SaveProgress();
		void LoadProgress();
		void SelectShip(string id);
		void SelectDifficulty(string id);
		void SelectLevel(string id);
		void Play();
		void Exit();
	}
}


/*
Button MainMenu = new NavButton(currMenu);
Button.Update()






class NavButton : IMenuModuleController {
	
}

*/
