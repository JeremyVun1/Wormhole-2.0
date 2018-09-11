using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Wormhole
{
	public class Emitter : Component
	{
		private float emissionRate;

		public override void Init(dynamic obj)
		{
			id = obj.id;
			emissionRate = obj.rate;
			Mass = 0;
			scale = 0;
			shape = null;

			ChildComponents = new ParticleGroup(obj.particle);
		}
	}
}
