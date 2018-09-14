using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public class ParticleHandler
	{
		//TODO create a common interface for spawned entities (ammo, particles)
		//reference to parent shooter
		private IShoots parentShooter;
		private List<Particle> particleList;

		public ParticleHandler(IShoots shooter)
		{
			particleList = new List<Particle>();
			parentShooter = shooter;
		}

		public void AddParticle(Particle p)
		{
			Point2D spawnPos = p.Pos.Add(parentShooter.Pos);
			p.TeleportTo(spawnPos);

			Vector spawnDir = parentShooter.Dir;
			p.TurnTo(spawnDir);

			particleList.Add(p);
		}

		public void Draw()
		{
			foreach (Particle p in particleList)
			{
				p.Draw(SwinGame.PointAt(80, 80), Color.Brown);
			}
		}

		public void Update()
		{
			foreach (Particle p in particleList)
			{
				if (p.Dead)
					particleList.Remove(p);
			}
		}
	}
}
