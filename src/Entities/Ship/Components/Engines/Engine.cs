using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Wormhole
{
	public class Engine : Component
	{
		private float thrust;
		private float maxVel;
		private float turnRate;
		
		//public Emitter Emitter { get; set; }

		public override void Init(dynamic obj)
		{
			id = obj.id;
			thrust = obj.thrust;
			maxVel = obj.maxVel;
			turnRate = obj.turnRate;
			Mass = obj.mass;
			scale = obj.scale;
			shape = new Shape(obj.shape, scale);

			ChildComponents = new EmitterGroup(obj.emitters);
		}
	}
}
