using System;
using System.Collections.Generic;
using Stateless;
using SwinGameSDK;
using Wormhole.Delegates;

namespace Wormhole
{
	public class MenuModule : IMenuModule
	{
		//state machine stuff
		//public enum State { };
		//public enum Trigger { CHANGE};
		//StateMachine<Menu.State, Menu.Trigger> stateMachine;

		private Dictionary<string, Menu> menus;
		public Menu currMenu { get; private set; }
		public Player PlayerProgress { get; set; }
		private ShipList shipList;
		private LevelList levelList;
		public bool Ended { get; private set; }
		private Selections selections { get; set; }

		//delegate from game controller to exit the game
		ExitGame ExitDelegate;

		public MenuModule() { }
		public MenuModule(Player player, ShipList shipList, LevelList levelList, ExitGame Exit)
		{
			//1 money
			//2 ship list to display (dictionary<ship, boolean>)
			//3 player level progress
			//4 list of levels
			//5 difficulty options (and what the player has selected)

			ExitDelegate = Exit;
			selections = new Selections();
			menus = new Dictionary<string, Menu>(); //init menu dictionary
			UpdateProgress(player, shipList, levelList); //information for displaying menu according to player progress
		}

		public void Exit()
		{
			ExitDelegate();
		}

		public void UpdateProgress(Player player, ShipList shipList, LevelList levelList)
		{
			PlayerProgress = player;
			this.shipList = shipList;
			this.levelList = levelList;
		}

		public void AddMenu(string menuId, Menu menu)
		{
			if (menus.ContainsKey(menuId))
				menus[menuId] = menu;
			else
				menus.Add(menuId, menu);
		}

		public void ChangeMenu(string target)
		{
			if (menus.ContainsKey(target))
			{
				currMenu?.ResetButtons();
				currMenu = menus[target];
			}
			else Log.Msg(String.Format("Can't find menu of id: {0}", target));
		}

		public void Run()
		{
			Update();
			Draw();
		}

		public void Update()
		{
			currMenu.Update();
		}

		public void Draw()
		{
			currMenu.Draw();
		}

		public void LoadProgress()
		{
			//load progress from file here
		}

		public void SaveProgress()
		{
			//save progress to file here
		}

		public void SelectLevel(string levelId)
		{
			selections.SetLevel(levelId);
		}

		public void SelectShip(string shipId)
		{
			selections.SetShip(shipId);
		}

		public void SelectDifficulty(string diffId)
		{
			selections.SetDifficulty(diffId);
		}

		public Selections FetchSelections()
		{
			return selections;
		}
		
		public void Play()
		{
			//check if everything is selected
			if (selections.AllSelected())
			{
				Ended = true;
			}
			else
			{
				SwinGame.PlaySoundEffect("Error");
				currMenu.ResetButtons();
			}
		}
	}
}
