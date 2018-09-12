using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public static class UtilExtensions
	{
		public static Vector Rotate(this Vector v, double theta)
		{
			Vector result = SwinGame.VectorTo(0, 0);
			result.X = (float)(v.X * Math.Cos(theta) - v.Y * Math.Sin(theta));
			result.Y = (float)(v.Y * Math.Cos(theta) + v.X * Math.Sin(theta));

			return result;
		}

		//clamp for floats
		public static float Clamp(this float x, float min, float max)
		{
			if (x < min)
				return min;
			else if (x > max)
				return max;
			return x;
		}

		//clamp for ints
		public static int Clamp(this int x, int min, int max)
		{
			if (x < min)
				return min;
			else if (x > max)
				return max;
			return x;
		}

		public static float GetSign(this float x)
		{
			if (x < 0)
				return -1;
			if (x > 0)
				return 1;
			return 0;
		}

		public static void Draw(this LineSegment l, Color clr, Point2D offset)
		{
			LineSegment result = SwinGame.CreateLine(l.StartPoint.Add(offset), l.EndPoint.Add(offset));
			SwinGame.DrawLine(clr, result);
		}

		public static Point2D Multiply(this Point2D p, float m)
		{
			return SwinGame.PointAt(p.X * m, p.Y * m);
		}
	}
}
