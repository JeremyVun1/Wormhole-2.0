using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public class AmmoHandler
	{
		//reference to parent shooter
		private IShoots parentShooter;
		private List<Ammo> ammoList;

		public AmmoHandler(IShoots shooter)
		{
			ammoList = new List<Ammo>();
			parentShooter = shooter;
		}

		public void AddAmmo(Ammo a)
		{
			Console.WriteLine("add ammo called");
			Point2D spawnPos = a.Pos.Add(parentShooter.Pos);
			a.TeleportTo(spawnPos);

			Vector spawnDir = parentShooter.Dir;
			a.TurnTo(spawnDir);

			a.StartLifetime();
			ammoList.Add(a);
		}

		public void Draw()
		{
			foreach(Ammo a in ammoList)
			{
				a.Draw(SwinGame.PointAt(10, 10), Color.Green);
			}
		}

		public void Update()
		{
			//iterate backwards to remove entities
			for (int i = ammoList.Count-1; i >= 0; --i)
			{
				ammoList[i].Update();
				Log.Pos(ammoList[i].Pos);
				if (ammoList[i].Dead) {
					ammoList.Remove(ammoList[i]);
					Console.WriteLine("ammo removed");
				}
			}
		}
	}
}
