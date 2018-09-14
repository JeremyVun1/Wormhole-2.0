using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public interface IMoves
	{
		void Thrust(Vector dir);

		void TurnBy(float theta);

		void TurnTo(Vector dir);
	}
}
