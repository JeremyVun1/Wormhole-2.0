using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;

namespace TaskForceUltra
{
	public static class UtilExtensions
	{
		////////////////////
		// Math extensions
		////////////////////
		public static float Clamp(this float x, float min, float max) {
			if (x < min)
				return min;
			else if (x > max)
				return max;
			return x;
		}

		public static double Clamp(this double x, double min, double max) {
			if (x < min)
				return min;
			else if (x > max)
				return max;
			return x;
		}

		public static int Clamp(this int x, int min, int max) {
			if (x < min)
				return min;
			else if (x > max)
				return max;
			return x;
		}

		public static float GetSign(this float x) {
			if (x < 0)
				return -1;
			if (x > 0)
				return 1;
			return 0;
		}

		public static double GetSign(this double x) {
			if (x < 0)
				return -1;
			if (x > 0)
				return 1;
			return 0;
		}

		public static Point2D Multiply(this Point2D p, float x) {
			return SwinGame.PointAt(p.X * x, p.Y * x);
		}

		public static Point2D Subtract(this Point2D p1, Point2D p2) {
			return SwinGame.PointAt(p1.X - p2.X, p1.Y - p2.Y);
		}

		public static LineSegment Multiply(this LineSegment l, int x) {
			return SwinGame.CreateLine(l.StartPoint.Multiply(x), l.EndPoint.Multiply(x));
		}

		//////////////////////
		// drawing linesegment
		//////////////////////
		public static void Draw(this LineSegment l, Color clr, Point2D offset) {
			LineSegment result = SwinGame.CreateLine(l.StartPoint.Add(offset), l.EndPoint.Add(offset));
			SwinGame.DrawLine(clr, result);
		}

		//////////////////////////////////
		//component list extension methods
		//////////////////////////////////
		public static void TeleportTo(this List<Component> componentList, Point2D target) {
			foreach (Component c in componentList) {
				c?.TeleportTo(target);
			}
		}

		public static int Mass(this List<Component> componentList) {
			int result = 0;
			foreach(Component c in componentList) {
				if (c != null)
					result += c.Mass;
			}
			return result;
		}

		public static void Update(this List<Component> componentList) {
			foreach (Component c in componentList) {
				c?.Update();
			}
		}

		public static void Draw(this List<Component> componentList) {
			foreach (Component c in componentList) {
				c?.Draw();
			}
		}

		public static void SetVel(this List<Component> componentList, Vector v) {
			foreach (Component c in componentList) {
				c?.SetVel(v);
			}
		}

		public static void Turn(this List<Component> componentList, double theta) {
			foreach (Component c in componentList) {
				c?.Turn(theta);
			}
		}
	}
}
