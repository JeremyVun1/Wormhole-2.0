using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class SelectDifficultyCommand : ICommand
	{
		MenuModule menuModule;
		string diffId;

		public SelectDifficultyCommand(MenuModule m, string id)
		{
			menuModule = m;
			diffId = id;
		}

		public void Execute()
		{
			menuModule.SelectDifficulty(diffId);
		}
	}
}
