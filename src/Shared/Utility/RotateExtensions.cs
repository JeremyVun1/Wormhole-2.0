using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra
{
	public static class RotateExtensions
	{
		///////////////
		// Rotation math
		///////////////
		public static Vector Rotate(this Vector v, double theta) {
			Vector result = SwinGame.VectorTo(0, 0);
			result.X = (float)(v.X * Math.Cos(theta) - v.Y * Math.Sin(theta));
			result.Y = (float)(v.Y * Math.Cos(theta) + v.X * Math.Sin(theta));

			return result;
		}

		public static Point2D Rotate(this Point2D p, Point2D pos, double theta) {
			//rotate by formula
			//x'=x*Cos(t) - y*Sin(t)
			//y'=y*Cos(t) + x*Sin(t)

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

		public static LineSegment Rotate(this LineSegment l, Point2D pos, double theta) {
			LineSegment result = l;
			result.StartPoint = result.StartPoint.Rotate(pos, theta);
			result.EndPoint = result.EndPoint.Rotate(pos, theta);

			return result;
		}

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
