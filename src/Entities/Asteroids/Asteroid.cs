using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SwinGameSDK;

namespace Wormhole
{
	public class Asteroid : Entity
	{
		public Asteroid(string id, int health, dynamic rgb, dynamic shape, dynamic scale) : base(id, health, (JArray)rgb, (JObject)shape, (float)scale)
		{
		}

		public override void ReactToCollision(int dmg, Point2D vel, Vector dir)
		{
			throw new NotImplementedException();
		}
	}
}
