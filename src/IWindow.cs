namespace Wormhole
{
	public interface IWindow
	{
		Size2D<int> WindowSize { get; }
		string WindowTitle { get; }

		void SetWindow(int w, int h);

		void Update();

		void Draw();
	}
}