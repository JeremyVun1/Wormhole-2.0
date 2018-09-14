using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class PlayCommand : ICommand
	{
		MenuModule menuModule;

		public PlayCommand(MenuModule m)
		{
			menuModule = m;
		}

		public void Execute()
		{
			menuModule.Play();
		}
	}
}
