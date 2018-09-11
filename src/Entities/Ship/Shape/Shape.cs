using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class Shape
	{
		private List<LineSegment> shape;
		private float length;

		public int Mass
		{
			get { return shape.Count; }
		}

		public Shape(dynamic s, float scale)
		{
			shape = new List<LineSegment>();
			length = 10 * scale;

			if (s.lines != null)
				AddLines(s.lines.ToObject<List<LineSegment>>());
			if (s.boxes != null)
				AddBoxes(s.boxes.ToObject<List<Point2D>>());
			if (s.triangles != null)
				AddTriangles(s.triangles.ToObject<List<Point2D>>());
		}

		private void AddLine(LineSegment l)
		{
			shape.Add(l);
		}

		private void AddLines(List<LineSegment> lines)
		{
			foreach(LineSegment l in lines)
			{
				LineSegment result = l;
				result.StartPoint = l.StartPoint.Multiply(length);
				result.EndPoint = l.EndPoint.Multiply(length);
				AddLine(l);
			}
		}

		private void AddBoxes(List<Point2D> pos)
		{
			foreach(Point2D p in pos)
			{
				Point2D[] corners = BoxCorners(p, length);
				AddLine(SwinGame.CreateLine(corners[0], corners[1]));
				AddLine(SwinGame.CreateLine(corners[1], corners[2]));
				AddLine(SwinGame.CreateLine(corners[2], corners[3]));
				AddLine(SwinGame.CreateLine(corners[3], corners[0]));
			}
		}

		private void AddTriangles(List<Point2D> pos)
		{
			foreach(Point2D p in pos)
			{
				Point2D[] corners = BoxCorners(p, length);
				AddLine(SwinGame.CreateLine(corners[3], corners[2]));
				AddLine(SwinGame.CreateLine(corners[3].X, corners[3].Y, corners[1].X - corners[0].X, corners[1].Y));
				AddLine(SwinGame.CreateLine(corners[2].X, corners[2].Y, corners[1].X - corners[0].X, corners[1].Y));
			}
		}

		public void Draw(Point2D pos, Color clr)
		{
			foreach(LineSegment l in shape)
			{
				l.Draw(clr, pos);
			}
		}

		private Point2D[] BoxCorners(Point2D pos, float length)
		{
			return new Point2D[4]
			{
				pos.Multiply(length),
				SwinGame.PointAt(pos.Multiply(length).X + length, pos.Multiply(length).Y),
				SwinGame.PointAt(pos.Multiply(length).X + length, pos.Multiply(length).Y + length),
				SwinGame.PointAt(pos.Multiply(length).X, pos.Multiply(length).Y + length)
			};
		}
	}
}
