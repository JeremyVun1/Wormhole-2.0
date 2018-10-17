using System;
using SwinGameSDK;
using TaskForceUltra.src;

namespace TaskForceUltra
{
	public class GameMain
	{
		public static void Main()
		{
			DoubtfireConcat concat = new DoubtfireConcat();
			concat.Run();

			GameController TaskForceUltra = new GameController();
			TaskForceUltra.Run();
		}
	}
}