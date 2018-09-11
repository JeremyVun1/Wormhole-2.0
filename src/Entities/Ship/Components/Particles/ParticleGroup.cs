using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class ParticleGroup : ComponentGroup
	{
		protected string particleDir;

		public ParticleGroup(dynamic particlesJArr) : base()
		{
			particleDir = resourcePath + "\\particles";

			components.Add(particlesJArr.ToObject<Particle>());

			InitComponents();
		}
	}
}
