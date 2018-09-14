using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public interface IHasPowerups
	{
		void ActivatePowerup(PowerupType powerup);
	}
}
