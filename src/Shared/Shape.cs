using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaskForceUltra.src.GameModule;

namespace TaskForceUltra
{
	public class Shape
	{
		private List<LineSegment> shape;
		private float length;
		private Point2D pos;
		public List<LineSegment> BoundingBox { get; private set; }
		public int Mass {
			get { return shape.Count; }
		}

		public Shape(List<LineSegment> shape, List<LineSegment> boundingBox, int length, Point2D offsetPos) {
			this.shape = shape;
			BoundingBox = boundingBox;
			this.length = length;
			pos = offsetPos; //should we be creating at origin? test this
		}

		public void TeleportTo(Point2D target) {
			Vector moveBy = target.Add(pos.Multiply(-1));
			Move(moveBy);
			pos = target;
		}

		public void Move(Vector v) {
			BoundingBox = BoundingBox?.Move(v);
			pos += v;
		}

		public void Rotate(double theta) {
			shape = shape?.Rotate(SwinGame.PointAt(0, 0), theta);
			BoundingBox = BoundingBox?.Rotate(pos, theta);
		}

		public void Draw(Color clr) {
			foreach (LineSegment l in shape) {
				l.Draw(clr, pos);
			}
		}

		public void Debug(Color clr) {
			if (BoundingBox == null)
				return;

			foreach (LineSegment l in BoundingBox) {
				SwinGame.DrawLine(clr, l);
			}
		}

		public List<LineSegment> GetLines() {
			return shape;
		}
	}

	/// <summary>
	/// Shape Factory
	/// </summary>
	public class ShapeFactory
	{
		public Shape Create(JObject shapeObj, float s, Point2D offsetPos) {
			if (shapeObj == null)
				return null;

			float scale = (s == 0 ? 0.1f : s); //don't allow 0 scale
			int lineLength = (int)(10 * scale);

			List<LineSegment> shape = new List<LineSegment>();
			List<LineSegment> boundingBox = new List<LineSegment>();

			JArray linesObj = shapeObj.Value<JArray>("lines");
			JArray boxesObj = shapeObj.Value<JArray>("boxes");
			JArray trianglesObj = shapeObj.Value<JArray>("triangles");

			AddLines(shape, linesObj, lineLength);
			AddBoxes(shape, boxesObj, lineLength);
			AddTriangles(shape, trianglesObj, lineLength);
			boundingBox = CreateBoundingBox(shape);

			shape = shape.Move(offsetPos);
			shape = shape.Move(SwinGame.PointAt(-lineLength / 2, -lineLength / 2));

			Shape result = new Shape(shape, boundingBox, lineLength, offsetPos);

			return result;
		}

		//Methods for deserialising lines, boxes, triangles into line segments
		private void AddLines(List<LineSegment> shape, JArray linesObj, int lineLength) {
			if (linesObj == null)
				return;

			List<LineSegment> lines = linesObj.ToObject<List<LineSegment>>();

			for (int i = 0; i < lines.Count; ++i) {
				shape.Add(lines[i].Multiply(lineLength));
			}
		}

		private void AddBoxes(List<LineSegment> shape, JArray boxesObj, int lineLength) {
			if (boxesObj == null)
				return;

			List<Point2D> boxes = boxesObj.ToObject<List<Point2D>>();
			foreach (Point2D p in boxes) {
				Point2D[] corners = BoxCorners(p, lineLength);
				shape.Add(SwinGame.CreateLine(corners[0], corners[1]));
				shape.Add(SwinGame.CreateLine(corners[1], corners[2]));
				shape.Add(SwinGame.CreateLine(corners[2], corners[3]));
				shape.Add(SwinGame.CreateLine(corners[3], corners[0]));
			}
		}

		private void AddTriangles(List<LineSegment> shape, JArray trianglesObj, int lineLength) {
			if (trianglesObj == null)
				return;

			List<Point2D> triangles = trianglesObj.ToObject<List<Point2D>>();

			foreach (Point2D p in triangles) {
				Point2D[] corners = BoxCorners(p, lineLength);
				LineSegment bottom = SwinGame.CreateLine(corners[3], corners[2]);
				LineSegment left = SwinGame.CreateLine(corners[2], SwinGame.LineMidPoint(SwinGame.CreateLine(corners[0], corners[1])));
				LineSegment right = SwinGame.CreateLine(corners[3], SwinGame.LineMidPoint(SwinGame.CreateLine(corners[0], corners[1])));

				shape.Add(bottom);
				shape.Add(left);
				shape.Add(right);
			}
		}

		private Point2D[] BoxCorners(Point2D pos, float length) {
			return new Point2D[4]
			{
				pos.Multiply(length),
				SwinGame.PointAt(pos.Multiply(length).X + length, pos.Multiply(length).Y),
				SwinGame.PointAt(pos.Multiply(length).X + length, pos.Multiply(length).Y + length),
				SwinGame.PointAt(pos.Multiply(length).X, pos.Multiply(length).Y + length)
			};
		}

		//build bounding box for collision mask
		private List<LineSegment> CreateBoundingBox(List<LineSegment> lines) {
			float xMin = 0, xMax = 0, yMin = 0, yMax = 0;

			foreach (LineSegment l in lines) {
				FindFromPoint2D(l.StartPoint);
				FindFromPoint2D(l.EndPoint);
			}

			void FindFromPoint2D(Point2D linePos) {
				if (linePos.X < xMin)
					xMin = linePos.X;
				if (linePos.X > xMax)
					xMax = linePos.X;
				if (linePos.Y < yMin)
					yMin = linePos.Y;
				if (linePos.Y > yMax)
					yMax = linePos.Y;
			}

			return new List<LineSegment>() {
				SwinGame.CreateLine(xMin, yMin, xMax, yMin),
				SwinGame.CreateLine(xMax, yMin, xMax, yMax),
				SwinGame.CreateLine(xMax, yMax, xMin, yMax),
				SwinGame.CreateLine(xMin, yMax, xMin, yMin)
			};
		}
	}
}
