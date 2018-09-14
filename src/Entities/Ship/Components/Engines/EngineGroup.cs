using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Wormhole
{
	public class EngineGroup : ComponentGroup
	{
		protected string engineDir;

		public EngineGroup(dynamic enginesJArr, ParticleHandler handler) : base()
		{
			engineDir = resourcePath + "\\entities\\engines";

			foreach (JObject e in enginesJArr)
			{
				Engine result = new Engine(handler);

				string json = JsonConvert.SerializeObject(e);
				JsonConvert.PopulateObject(json, result);

				components.Add(result);
			}

			InitComponents();

			Console.WriteLine("AHHHHHHHH");
		}

		public Vector ApplyThrust (Vector vDir, Vector vel, int Mass)
		{
			foreach (Engine e in components.OfType<Engine>())
			{
				vel = e.ApplyForce(vDir, vel, Mass);
			}

			return vel;
		}
	}
}
