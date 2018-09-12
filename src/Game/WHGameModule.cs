using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class WHGameModule : IGameModule
	{
		public Player PlayerProgress { get; private set; }
		private IShip playerShip { get; set; }
		private Level Level { get; set; }

		public bool Ended { get; }

		public WHGameModule(Player p, IShip s, Level l)
		{
			UpdateProgress(p, s, l);
		}

		public void UpdateProgress(Player p, IShip s, Level l)
		{
			PlayerProgress = p;
			playerShip = s;
			Level = l;
		}

		public void Draw()
		{
			Level.Draw();
			playerShip.Draw();
		}

		public bool IsEnded()
		{
			return false;
		}

		public void Update()
		{
			Level.Update();
			playerShip.Update();
		}

		//device controls the ship
		//use command pattern
	}
}
