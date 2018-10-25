using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra
{
	/// <summary>
	/// Extension methods for movements
	/// </summary>
	public static class MoveExtensions
	{
		/// <summary>
		/// Move a point
		/// </summary>
		/// <param name="p">point</param>
		/// <param name="vel">velocity</param>
		/// <returns>Moved Point</returns>
		public static Point2D Move(this Point2D p, Vector vel) {
			Point2D result = p;
			return result += vel;
		}

		/// <summary>
		/// Move a line segment
		/// </summary>
		/// <param name="l">linesegment</param>
		/// <param name="vel">velocity</param>
		/// <returns>Moved Line segemtn</returns>
		public static LineSegment Move(this LineSegment l, Vector vel) {
			LineSegment result = l;
			result.StartPoint = result.StartPoint.Move(vel);
			result.EndPoint = result.EndPoint.Move(vel);

			return result;
		}

		/// <summary>
		/// Move a bunch of line segments
		/// </summary>
		/// <param name="s">list of line segments</param>
		/// <param name="vel">velocity</param>
		/// <returns>list of line segments</returns>
		public static List<LineSegment> Move(this List<LineSegment> s, Vector vel) {
			List<LineSegment> result = s;
			for (int i = 0; i < result.Count; ++i) {
				result[i] = result[i].Move(vel);
			}

			return result;
		}
	}
}
