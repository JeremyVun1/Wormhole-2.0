using System;
using System.Collections.Generic;
using Stateless;

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

		public bool Ended { get; }

		public MenuModule() { }
		public MenuModule(Player player, ShipList shipList, LevelList levelList)
		{
			//1 money
			//2 ship list to display (dictionary<ship, boolean>)
			//3 player level progress
			//4 list of levels
			//5 difficulty options (and what the player has selected)

			menus = new Dictionary<string, Menu>(); //init menu dictionary
			UpdateProgress(player, shipList, levelList); //information for displaying menu according to player progress


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
				currMenu = menus[target];
			else Console.WriteLine("I CANT FIND THE MENU NAMED " + target);
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

		public bool IsEnded()
		{
			throw new NotImplementedException();
		}
	}
}
