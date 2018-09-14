using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public class WHGameModule : IGameModule
	{
		public Player PlayerProgress { get; private set; }
		private Level level;
		private GameInputHandler inputHandler;
		private CameraHandler cameraHandler;
		private ActionBindingFactory bindingFac;

		/////////////////////////////
		//what to get from the game
		//money
		//ship status
		//level status/score

		public bool Ended
		{
			get { return level.Ended; }
		}

		public WHGameModule(Player p, CameraHandler camHandler, GameInputHandler inpHandler, Level lvl)
		{
			PlayerProgress = p;

			//sub modules that can be updated
			level = lvl;
			cameraHandler = camHandler;
			inputHandler = inpHandler;
		}

		public void Draw()
		{
			level.Draw();
		}

		public void Update()
		{
			level.Update();
			inputHandler.Update();
			cameraHandler.Update();
		}
	}
}
