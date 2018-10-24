using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra
{
	/// <summary>
	/// Extension methods for rotating stuff
	/// </summary>
	public static class RotateExtensions
	{
		/// <summary>
		/// Rotate Vector
		/// </summary>
		/// <param name="v">Vector</param>
		/// <param name="theta">radians</param>
		public static Vector Rotate(this Vector v, double theta) {
			Vector result = SwinGame.VectorTo(0, 0);
			result.X = (float)(v.X * Math.Cos(theta) - v.Y * Math.Sin(theta));
			result.Y = (float)(v.Y * Math.Cos(theta) + v.X * Math.Sin(theta));

			return result;
		}

		/// <summary>
		/// Rotate a point around an anchor of rotation
		/// </summary>
		/// <param name="p">point</param>
		/// <param name="pos">anchor of rotation</param>
		/// <param name="theta">radians</param>
		public static Point2D Rotate(this Point2D p, Point2D pos, double theta) {
			Point2D temp = new Point2D {
				X = p.X - pos.X,
				Y = p.Y - pos.Y
			};

			Point2D result = new Point2D {
				X = (float)(temp.X * Math.Cos(theta) - temp.Y * Math.Sin(theta) + pos.X),
				Y = (float)(temp.Y * Math.Cos(theta) + temp.X * Math.Sin(theta) + pos.Y)
			};

			return result;
		}

		/// <summary>
		/// Rotate Linesegment around an anchor of rotation
		/// </summary>
		/// <param name="l">linesegment</param>
		/// <param name="pos">anchor of rotation</param>
		/// <param name="theta">radians</param>
		public static LineSegment Rotate(this LineSegment l, Point2D pos, double theta) {
			LineSegment result = l;
			result.StartPoint = result.StartPoint.Rotate(pos, theta);
			result.EndPoint = result.EndPoint.Rotate(pos, theta);

			return result;
		}

		/// <summary>
		/// Rotate List of line segments
		/// </summary>
		/// <param name="s">list of linesegments</param>
		/// <param name="pos">anchor of rotation</param>
		/// <param name="theta">radians</param>
		public static List<LineSegment> Rotate(this List<LineSegment> s, Point2D pos, double theta) {
			if (s == null) return null;

			List<LineSegment> result = s;
			for (int i = 0; i < result.Count; ++i) {
				result[i] = result[i].Rotate(pos, theta);
			}

			return result;
		}
	}
}
