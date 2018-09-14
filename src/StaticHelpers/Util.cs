using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public static class Util
	{
		public static float RandomRange(MinMax<float> x)
		{
			Random rng = new Random(Guid.NewGuid().GetHashCode());

			return rng.Next((int)(x.Min * 1000), (int)(x.Max * 1000));
		}
	}
}
