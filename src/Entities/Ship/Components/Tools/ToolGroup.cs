using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class ToolGroup : ComponentGroup
	{
		protected string toolDir;

		public ToolGroup(dynamic toolsJArr) : base()
		{
			toolDir = resourcePath + "\\tools";

			//deserialise the json object into a list of component skeletons
			List<Tool> temp = toolsJArr.ToObject<List<Tool>>();
			components = temp.ToList<IComponent>();

			InitComponents();
		}
	}
}
