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
	/// <summary>
	/// Manages the shape of the parent object as a bunch of line segments
	/// </summary>
	public class Shape
	{
		private List<LineSegment> shape;
		private Point2D pos;
		public List<LineSegment> BoundingBox { get; private set; }
		public int Mass {
			get { return shape.Count * 3; }
		}

		public Shape(List<LineSegment> shape, List<LineSegment> boundingBox, Point2D offsetPos) {
			this.shape = shape;
			BoundingBox = boundingBox;
			pos = offsetPos;
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

		/// <summary>
		/// Returns line segments that it is managing
		/// </summary>
		/// <returns>List of line segments</returns>
		public List<LineSegment> GetLines(int n) {
			if (n == 0)
				return null;

			n = n > shape.Count ? shape.Count-1 : n-1;
			return shape.GetRange(0,n);
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
		public Shape CreateCircleApprox(float length, int edges) {
			MinMax<float> angleRange = new MinMax<float>(0.3f, 1.5f);
			edges = Math.Max(edges, 3);
			float[] angles = new float[edges];

			//use random angles
			angles = BuildCircleApproxAngles(edges, angleRange);

			//create lines
			List<LineSegment> lines = new List<LineSegment>();
			Point2D lastPoint = SwinGame.PointAt(0, 0);
			float totalAngle = 0;
			foreach (float angle in angles) {
				totalAngle += angle;
				Vector vec = SwinGame.VectorFromAngle(totalAngle, length);
				lines.Add(SwinGame.CreateLine(lastPoint, lastPoint + vec));
				lastPoint = lines.Last().EndPoint;
			}
			lines.Add(SwinGame.CreateLine(lastPoint, SwinGame.PointAt(0, 0)));

			//bounding box
			List<LineSegment> boundingBox = CreateBoundingBox(lines);
			Shape shape = new Shape(lines, boundingBox, SwinGame.PointAt(0, 0));

			return shape;
		}

		private float[] BuildCircleApproxAngles(int edges, MinMax<float> angleRange) {
			float totalAngle = 0;
			float[] result = new float[edges];

			for (int i = 0; i < edges-1; i++) {
				float angle = (360 / edges) * Util.RandomInRange(angleRange);

				//check that we are not exceeding 360
				angle = totalAngle + angle < 360 ? angle : 360 - totalAngle;
				if (angle <= 0)
					break;
				result[i] = angle;
				totalAngle += angle;
			}

			if (totalAngle < 360)
				result[edges-1] = 360 - totalAngle;

			return result;
		}

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

			return new Shape(shape, boundingBox, offsetPos);
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
