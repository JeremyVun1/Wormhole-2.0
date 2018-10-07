namespace TaskForceUltra
{
	public struct MinMax<T>
	{
		public T Min { get; set; }
		public T Max { get; set; }

		public MinMax(T min, T max) {
			Min = min;
			Max = max;
		}
	}
}
