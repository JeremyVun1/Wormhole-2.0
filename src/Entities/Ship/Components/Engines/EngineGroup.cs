using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class EngineGroup : ComponentGroup
	{
		protected string engineDir;

		public EngineGroup(dynamic enginesJArr) : base()
		{
			engineDir = resourcePath + "\\engines";

			//deserialise the json object into a list of component skeletons
			List<Engine> temp = enginesJArr.ToObject<List<Engine>>();
			components = temp.ToList<IComponent>();

			InitComponents();
		}
	}
}
