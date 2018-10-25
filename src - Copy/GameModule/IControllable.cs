using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Receiver interface for a controllable game object
	/// </summary>
	public interface IControllable
	{
		void ForwardCommand();
		void BackwardCommand();
		void StrafeLeftCommand();
		void StrafeRightCommand();
		void TurnRightCommand();
		void TurnLeftCommand();
		void ShootCommand();
		void ActivatePowerupCommand();
	}
}
