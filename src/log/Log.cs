using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public static class Log
	{
		//exceptions
		public static void Ex(Exception e, string desc)
		{
			Console.WriteLine(desc + e.Message);
		}

		//log points
		public static void Pos(Point2D pos)
		{
			Msg("x: " + pos.X + " y: " + pos.Y);
		}

		public static void Pos(string id, Point2D pos)
		{
			Console.Write("id: " + id + " ");
			Pos(pos);
		}

		//log vectors
		public static void Vec(Vector vec)
		{
			Msg("x: " + vec.X + " y: " + vec.Y);
		}

		public static void Vec(string id, Vector vec)
		{
			Console.Write("id: " + id + " ");
			Vec(vec);
		}

		//log booleans
		public static void True()
		{
			Msg("True");
		}
		public static void False()
		{
			Msg("False");
		}

		//log message
		public static void Msg(string line)
		{
			Console.WriteLine(line);
		}
	}
}
