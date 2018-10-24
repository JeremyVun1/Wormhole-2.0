namespace TaskForceUltra
{
	/// <summary>
	/// struct that defines width and height
	/// </summary>
	public struct Size2D<T>
	{
		public T W { get; set; }
		public T H { get; set; }

		public Size2D(T w, T h) {
			W = w;
			H = h;
		}
	}
}
