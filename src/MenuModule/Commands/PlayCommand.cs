using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to start playing the game
	/// </summary>
	public class PlayCommand : ICommand
	{
		private IMenuModule menuModule;

		public PlayCommand(IMenuModule m) {
			menuModule = m;
		}

		public void Execute() {
			menuModule.Send();
		}
	}
}
