using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class ActivatePowerupCommand : ICommand
	{
		IControllable controlled;

		public ActivatePowerupCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.ActivatePowerupCommand();
		}
	}
}
