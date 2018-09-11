using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public struct MinMax<T>
	{
		public T Min { get; set; }
		public T Max { get; set; }

		public MinMax(T min, T max)
		{
			Min = min;
			Max = max;
		}
	}
}
