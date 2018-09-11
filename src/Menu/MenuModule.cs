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

		private ButtonHandler btnHandler;

		private Dictionary<string, Menu> menus;
		public Menu currMenu { get; private set; }
		public Player PlayerProgress { get; set; }

		public bool Ended { get; }

		public MenuModule() { }
		public MenuModule(Player player, ShipList shipList, LevelList levelList)
		{
			//1 money
			//2 ship list to display (dictionary<ship, boolean>)
			//3 player level progress
			//4 list of levels
			//5 difficulty options (and what the player has selected)
			//initialise
			menus = new Dictionary<string, Menu>();
			UpdateProgress(player, shipList, levelList);

			btnHandler = new ButtonHandler();

			ChangeMenu("main");
		}

		public void UpdateProgress(Player player, ShipList shipList, LevelList levelList)
		{

		}

		public void AddMenu(string key, Menu value)
		{
			if (menus.ContainsKey(key))
				menus[key] = value;
			else
				menus.Add(key, value);
		}

		public void ChangeMenu(string target)
		{
			if (menus.ContainsKey(target))
				currMenu = menus[target];
			else Log.Msg("I CANT FIND THE MENU OMG NPE");
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
