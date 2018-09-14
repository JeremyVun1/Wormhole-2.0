using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public class WHGameFactory
	{
		string dirPath;
		public WHGameFactory(string dir)
		{
			dirPath = dir;
		}

		//create new module
		public IGameModule Create(Player player, IControllableShip ship, Level level, DifficultyType difficulty = 0)
		{
			ActionBindingFactory abfac = new ActionBindingFactory(SwinGame.AppPath() + "\\resources\\settings");
			GameInputHandler inputHandler = new GameInputHandler(ship, abfac.Fetch(ControllerType.Player1));

			CameraHandler camHandler = new CameraHandler(ship, level.PlaySize);

			level.AddEntity(ship as Entity);
			level.SetDifficulty(difficulty);

			IGameModule result = new WHGameModule(player, camHandler, inputHandler, level);

			return result;
		}
	}
}
