namespace Wormhole
{
	public interface IShip
	{
		int Cost { get; }
		int Mass { get; }

		int RepairCost();

		void Draw();
		void Update();
	}
}