namespace Wormhole
{
	public interface IShip
	{
		string Id { get; }
		int Cost { get; }
		int Mass { get; }
		float Condition { get; }

		int RepairCost();

		void Draw();
		void Update();
	}
}