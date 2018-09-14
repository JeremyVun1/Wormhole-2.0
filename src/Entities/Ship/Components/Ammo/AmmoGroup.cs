using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class AmmoGroup : ComponentGroup
	{
		private string ammoDir;

		public AmmoGroup(dynamic ammoJArr) : base()
		{
			//ammo path
			ammoDir = resourcePath + "entities\\ammo";
			
			components.Add(ammoJArr.ToObject<Ammo>());

			InitComponents();
		}

		public string FetchAmmoId()
		{
			Ammo result = (Ammo)components.Find(x => x is Ammo);
			return result.Id;
		}
	}
}
