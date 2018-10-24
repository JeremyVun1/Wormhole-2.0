namespace TaskForceUltra
{
	public interface IMenuModule
	{
		void AddSelection(SelectionType selection, string id);
		void ChangeMenu(string id);
		void Exit();
		void RemoveSelection(SelectionType selection);
		void Send();
	}
}