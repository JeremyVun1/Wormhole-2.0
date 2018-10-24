using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to increase swingame music volume
	/// </summary>
	public class IncreaseVolumeCommand : ICommand
	{
		public IncreaseVolumeCommand() { }

		public void Execute() {
			SwinGame.SetMusicVolume(SwinGame.MusicVolume() + 0.1f);
		}
	}
}
