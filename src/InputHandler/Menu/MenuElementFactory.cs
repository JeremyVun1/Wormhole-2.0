using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public abstract class MenuElementFactory
	{
		public abstract IMenuElement Create(JToken element, JArray colors);

		public List<IMenuElement> CreateList(JArray elementArray, JArray colors)
		{
			List<IMenuElement> result = new List<IMenuElement>();

			foreach (var e in elementArray)
			{
				result.Add(Create(e, colors));
			}

			return result;
		}
	}
}
