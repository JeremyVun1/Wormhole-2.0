using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class SelectLevelCommand : ICommand
	{
		MenuModule menuModule;
		string levelId;

		public SelectLevelCommand(MenuModule m, string id)
		{
			menuModule = m;
			levelId = id;
		}

		public void Execute()
		{
			menuModule.SelectLevel(levelId);
		}
	}
}
