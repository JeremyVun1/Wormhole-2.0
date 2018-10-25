using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class TurnRightCommand : ICommand
	{
		IControllable controlled;

		public TurnRightCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.TurnRightCommand();
		}

		public void Undo() {
			controlled.TurnLeftCommand();
		}
	}
}
