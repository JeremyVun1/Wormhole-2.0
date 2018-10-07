using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule
{
	public interface IActionBinding
	{
		bool ActivatePowerup();
		bool Backward();
		bool Forward();
		bool Shoot();
		bool StrafeLeft();
		bool StrafeRight();
		bool TurnLeft();
		bool TurnRight();
	}
}
