using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class EmitterGroup : ComponentGroup
	{
		protected string emitterDir;

		public EmitterGroup(dynamic emittersJArr, ParticleHandler handler) : base()
		{
			emitterDir = resourcePath + "\\entities\\emitters";

			foreach (JObject e in emittersJArr)
			{
				Emitter result = new Emitter(handler);

				string json = JsonConvert.SerializeObject(e);
				JsonConvert.PopulateObject(json, result);

				components.Add(result);
			}

			InitComponents();

			Console.WriteLine("hi");
		}
	}
}
