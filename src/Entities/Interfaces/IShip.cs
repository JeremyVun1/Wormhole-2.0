using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public interface IShip : IMoves, IShoots, ITeleports, IHasPowerups, IHasHealth, IHasTeam, ISelfDestructs, IHasCost, IRepairable
	{
	}
}
