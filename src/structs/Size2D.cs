using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public struct Size2D<T>
	{
		public T W { get; set; }
		public T H { get; set; }

		public Size2D(T w, T h)
		{
			W = w;
			H = h;
		}
	}
}
