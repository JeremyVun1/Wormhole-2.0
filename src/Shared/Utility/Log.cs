using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra
{
	public static class Log
	{
		//log points
		public static void Pos(Point2D pos) {
			Console.WriteLine($"x: {pos.X} + y: {pos.Y}");
		}

		public static void Pos(string id, Point2D pos) {
			Console.Write($"id: {id} ");
			Pos(pos);
		}

		//log vectors
		public static void Vec(Vector vec) {
			Console.WriteLine($"x: {vec.X} y: {vec.Y}");
		}

		public static void Vec(string id, Vector vec) {
			Console.Write($"id: {id} ");
			Vec(vec);
		}
	}
}
