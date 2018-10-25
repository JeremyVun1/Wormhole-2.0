using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// Command to decrease swingame music volume
	/// </summary>
	public class DecreaseVolumeCommand : ICommand
	{
		public DecreaseVolumeCommand() { }

		public void Execute() {
			SwinGame.SetMusicVolume(SwinGame.MusicVolume() - 0.1f);
		}

		public void Undo() {
			SwinGame.SetMusicVolume(SwinGame.MusicVolume() + 0.1f);
		}
	}
}
