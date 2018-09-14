using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public enum OwnerType { Computer, Player };
	public enum AIType { Static, Erratic, Chase };
	public enum ShipAction { ActivatePowerup, Forward, Backward, StrafeLeft, StrafeRight, TurnLeft, TurnRight, Shoot, numEnums };
	public enum DifficultyType { Novice, Intermediate, Expert };
	public enum ControllerType { Player1, Player2, Player3, Player4 };
	public enum FactoryType { PlayerShip, AIShip, Shape, MovementAttributes, ActionBinding };
	public enum BtnAction { NavTo, Exit, Save };
	public enum EnvMods { Blackhole, Nebula, Radioactive };
	public enum MenuType { Main, Help, Highscores, Options, ScoreScreen, Select};
	public enum Team { Team1, Team2, Team3, Team4};
	public enum PowerupType { Speed, Damage, Invulnerability };
}
