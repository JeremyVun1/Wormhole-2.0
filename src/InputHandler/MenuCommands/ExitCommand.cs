using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class ExitCommand : ICommand {

		private MenuModule menuModule;

		public ExitCommand(MenuModule m)
		{
			menuModule = m;
		}

		public void Execute()
		{
			menuModule.Exit();
		}
	}
}
