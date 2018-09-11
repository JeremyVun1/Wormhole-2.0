using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public abstract class Mob : Entity
	{
		protected int BaseHealth { get; set; }
		protected int Health { get; set; }

		public Mob(string id, int health, JArray rgb, JObject shape, float scale) : base(id, rgb, shape, scale)
		{
			BaseHealth = health;
			Health = health;
		}
	}
}
