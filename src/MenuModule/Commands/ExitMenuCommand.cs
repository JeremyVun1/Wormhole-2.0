using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	public class ExitMenuCommand : ICommand
	{
		private IMenuModule menuModule;

		public ExitMenuCommand(IMenuModule m)
		{
			menuModule = m;
		}

		public void Execute() {
			menuModule.Exit();
		}
	}
}
