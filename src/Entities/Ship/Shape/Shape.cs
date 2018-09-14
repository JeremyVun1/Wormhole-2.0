﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class Shape : ITeleports
	{
		//We only move the bounding box to check for positional collisions
		//linesegments are always at origin for easier transformations
		//linesegments are always drawn at offset to the current ships position
		private List<LineSegment> shape;
		private float length;		
		public Rectangle BoundingBox { get; private set; }

		public int Mass
		{
			get { return shape.Count; }
		}

		public Shape(dynamic shapeJObj, float scale)
		{
			shape = new List<LineSegment>();
			length = 10 * scale;

			if (shapeJObj?.lines != null)
				AddLines(shapeJObj.lines.ToObject<List<LineSegment>>());
			if (shapeJObj?.boxes != null)
				AddBoxes(shapeJObj.boxes.ToObject<List<Point2D>>());
			if (shapeJObj?.triangles != null)
				AddTriangles(shapeJObj.triangles.ToObject<List<Point2D>>());

			UpdateBoundingBox(SwinGame.PointAt(0, 0));
		}

		public void TeleportTo(Point2D target)
		{
			Point2D currPos = BoundingBox.CenterLeft.Add(BoundingBox.CenterRight).Multiply(0.5f);
			Vector moveBy = target.Add(currPos.Multiply(-1));
			BoundingBox = BoundingBox.RectangleAfterMove(moveBy);
		}

		public void Move(Vector v)
		{
			BoundingBox = BoundingBox.RectangleAfterMove(v);
		}

		public void UpdateBoundingBox(Point2D pos)
		{
			float xMin = 0, xMax = 0, yMin = 0, yMax = 0;

			foreach(LineSegment l in shape)
			{
				FindFromPoint2D(l.StartPoint);
				FindFromPoint2D(l.EndPoint);
			}

			void FindFromPoint2D(Point2D linePos)
			{
				if (linePos.X < xMin)
					xMin = linePos.X;
				if (linePos.X > xMax)
					xMax = linePos.X;
				if (linePos.Y < yMin)
					yMin = linePos.Y;
				if (linePos.Y > yMax)
					yMax = linePos.Y;
			}

			BoundingBox = SwinGame.CreateRectangle(xMin, yMin, xMin + xMax, yMin + yMax);
			TeleportTo(pos);
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
