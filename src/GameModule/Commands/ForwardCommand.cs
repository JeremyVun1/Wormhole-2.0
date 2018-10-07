using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule.Commands
{
	public class ForwardCommand : ICommand
	{
		IControllable controlled;

		public ForwardCommand(IControllable c) {
			controlled = c;
		}

		public void Execute() {
			controlled.ForwardCommand();
		}
	}
}
