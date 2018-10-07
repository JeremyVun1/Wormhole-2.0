using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra
{
	public static class MoveExtensions
	{
		///////////////
		// Moving math
		///////////////
		public static Point2D Move(this Point2D p, Vector vel) {
			Point2D result = p;

			return result += vel;
		}

		public static LineSegment Move(this LineSegment l, Vector vel) {
			LineSegment result = l;
			result.StartPoint = result.StartPoint.Move(vel);
			result.EndPoint = result.EndPoint.Move(vel);

			return result;
		}

		public static List<LineSegment> Move(this List<LineSegment> s, Vector vel) {
			List<LineSegment> result = s;
			for (int i = 0; i < result.Count; ++i) {
				result[i] = result[i].Move(vel);
			}

			return result;
		}
	}
}
