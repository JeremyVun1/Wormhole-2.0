using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class LoadCommand : ICommand
	{
		MenuModule menuModule;

		public LoadCommand(MenuModule m)
		{
			menuModule = m;
		}

		public void Execute()
		{
			menuModule.LoadProgress();
		}
	}
}
