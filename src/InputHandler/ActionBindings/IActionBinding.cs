namespace Wormhole
{
	public interface IActionBinding
	{
		bool ActivatePowerup();
		bool Backward();
		bool Forward();
		bool Shoot();
		bool StrafeLeft();
		bool StrafeRight();
		bool TurnLeft();
		bool TurnRight();
	}
}