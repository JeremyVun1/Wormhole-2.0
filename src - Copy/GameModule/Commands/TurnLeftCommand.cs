using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class TurnLeftCommand : ICommand
	{
		IControllable controlled;

		public TurnLeftCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.TurnLeftCommand();
		}
	}
}
