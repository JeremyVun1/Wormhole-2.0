using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public interface IShoots
	{
		//attributes for spawning ammo in the correct position and direction
		Point2D Pos { get; }
		Vector Dir { get; }

		void Fire();
		void Fire(string weaponId);
	}
}
