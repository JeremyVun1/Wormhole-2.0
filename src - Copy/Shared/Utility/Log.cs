using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra
{
	/// <summary>
	/// logging stuff
	/// </summary>
	public static class Log
	{
		/// <summary>
		/// Log point to console
		/// </summary>
		/// <param name="pos">point</param>
		public static void Pos(Point2D pos) {
			Console.WriteLine($"x: {pos.X} + y: {pos.Y}");
		}

		/// <summary>
		/// Log point to console with label
		/// </summary>
		/// <param name="id">point name</param>
		/// <param name="pos">point</param>
		public static void Pos(string id, Point2D pos) {
			Console.Write($"id: {id} ");
			Pos(pos);
		}

		/// <summary>
		/// Log vector to console
		/// </summary>
		/// <param name="vec">Vector</param>
		public static void Vec(Vector vec) {
			Console.WriteLine($"x: {vec.X} y: {vec.Y}");
		}

		/// <summary>
		/// Log vector to console with label
		/// </summary>
		/// <param name="id">Vector name</param>
		/// <param name="vec">vector</param>
		public static void Vec(string id, Vector vec) {
			Console.Write($"id: {id} ");
			Vec(vec);
		}
	}
}
