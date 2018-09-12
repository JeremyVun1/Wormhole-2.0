namespace Wormhole
{
	public interface IMenuModule : IModule
	{
		Menu currMenu { get; }

		void AddMenu(string key, Menu value);
		void ChangeMenu(string target);
	}
}