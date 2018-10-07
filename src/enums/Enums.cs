using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra
{
	public enum SelectionType { Ship, Difficulty, Level }
	public enum EnvMod { Nebula, Flare, Blackhole, Void, Radioactive }
	public enum DifficultyType { Easy, Medium, Hard }
	public enum ShipAction { Forward, Backward, StrafeLeft, StrafeRight, TurnLeft, TurnRight, Shoot, ActivatePowerup }
	public enum ControllerType { Player1, Player2, Player3, Player4, Computer }
	public enum Team { Team1, Team2, Team3, Team4, Computer, None }
	public enum GameResultType { Time, Points, Result }
	public enum BattleResult { Loss = 0, Win = 1 }
}
