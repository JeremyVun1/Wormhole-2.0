using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class Ship : Mob, IShip
	{
		//general
		public int Cost { get; private set; }
		public float Condition
		{
			get { return (Health / BaseHealth); }
		}

		//components
		private ComponentGroup Engines { get; set; }
		private ComponentGroup Tools { get; set; }
		private ComponentGroup Emitters { get; set; }

		public Ship(dynamic obj) : base((string)obj.id, (int)obj.health, (JArray)obj.rgb, (JObject)obj.shape, (float)obj.scale)
		{
			Cost = obj.cost;
			
			Engines = new EngineGroup(obj.engines);
			Tools = new ToolGroup(obj.tools);
			Emitters = new EmitterGroup(obj.emitters);
		}

		//TODO
		//Split mass into it's own interface
		public int Mass
		{
			get { return Shape.Mass + Engines.Mass + Tools.Mass; }
		}

		public int RepairCost()
		{
			return Cost * (Health / BaseHealth);
		}
	}
}