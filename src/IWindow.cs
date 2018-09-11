namespace Wormhole
{
	public interface IWindow
	{
		Size2D<int> Size { get; }
		string Title { get; }

		void SetWindow(int w, int h);

		void Update();

		void Draw();
	}
}