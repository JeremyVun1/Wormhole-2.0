namespace Wormhole
{
	public interface IMenuModule : IModule, IControllableMenu
	{
		Menu currMenu { get; }

		void AddMenu(string key, Menu value);
	}
}