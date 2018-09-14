using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SwinGameSDK;
using Newtonsoft.Json;

namespace Wormhole
{
	public class ToolGroup : ComponentGroup
	{
		protected string toolDir;

		public ToolGroup(dynamic toolsJArr, AmmoHandler handler) : base()
		{
			toolDir = resourcePath + "entities\\tools";

			foreach(JObject t in toolsJArr)
			{
				Weapon result = new Weapon(handler);

				string json = JsonConvert.SerializeObject(t);
				JsonConvert.PopulateObject(json, result);

				components.Add(result);
			}

			InitComponents();

			Console.WriteLine("AHHH");
		}
	}
}
