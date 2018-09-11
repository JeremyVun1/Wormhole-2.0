using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class AmmoGroup : ComponentGroup
	{
		protected string ammoDir;

		public AmmoGroup(dynamic ammoJArr) : base()
		{
			ammoDir = resourcePath + "\\ammo";
			
			components.Add(ammoJArr.ToObject<Ammo>());

			InitComponents();
		}
	}
}
