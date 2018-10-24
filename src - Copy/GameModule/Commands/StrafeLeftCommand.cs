using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class StrafeLeftCommand : ICommand
	{
		IControllable controlled;

		public StrafeLeftCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.StrafeLeftCommand();
		}
	}
}
