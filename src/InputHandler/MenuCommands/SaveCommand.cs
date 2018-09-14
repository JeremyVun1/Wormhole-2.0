using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class SaveCommand : ICommand
	{
		MenuModule menuModule;

		public SaveCommand(MenuModule m)
		{
			menuModule = m;
		}

		public void Execute()
		{
			menuModule.SaveProgress();
		}
	}
}
