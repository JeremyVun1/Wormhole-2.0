using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;

namespace TaskForceUltra.src.GameModule.Handlers
{
	/// <summary>
	/// a tree node for storing entities within a specified area
	/// </summary>
	public class Node
	{
		private Node parent;
		private Node[] childNodes;
		private Rectangle grid;
		private int minWidth;
		public List<ICollides> ICollidesList { get; private set; } // list of all collideables
		public List<ICollides> CheckedList { get; private set; } // list of collideables that have already been collision checked

		public Node(Node parent, Rectangle grid, int minWidth) {
			this.parent = parent;
			this.grid = grid;
			this.minWidth = minWidth;
			ICollidesList = new List<ICollides>();
			CheckedList = new List<ICollides>();

			if (Math.Abs(grid.Width) > minWidth)
				CreateChildren();
		}

		public void DebugDraw() {
			if (DebugMode.IsDebugging(Debugging.Nodes)) {
				if (childNodes != null) {
					foreach (Node n in childNodes) {
						n.DebugDraw();
					}
				}
				SwinGame.DrawRectangle(Color.Yellow, grid);

				foreach(ICollides c in ICollidesList) {
					SwinGame.FillRectangle(Color.Aqua, c.RealPos.X, c.RealPos.Y, 10, 10);
				}

				if (ICollidesList.Count > 0) {
					SwinGame.FillRectangle(SwinGame.RGBAColor(255, 255, 0, 100), grid);
				}
			}
		}

		/// <summary>
		/// Recursive initialisation of the tree structure
		/// </summary>
		private void CreateChildren() {
			childNodes = new Node[4];
			Rectangle[] grids = CreateGrids();

			for(int i=0; i<4; ++i) {
				childNodes[i] = new Node(this, grids[i], minWidth);
			}
		}

		/// <summary>
		/// Split current grid into 4 children grids
		/// </summary>
		/// <returns>4 rectangles</returns>
		private Rectangle[] CreateGrids() {
			Point2D gridCenter = SwinGame.PointAt(grid.X + (grid.Width / 2), grid.Y + (grid.Height / 2));

			return new Rectangle[4] {
				SwinGame.CreateRectangle(grid.TopLeft, gridCenter), //top left
				SwinGame.CreateRectangle(grid.CenterTop, grid.CenterRight), //top right
				SwinGame.CreateRectangle(gridCenter, grid.BottomRight), //bottom right
				SwinGame.CreateRectangle(grid.CenterLeft, grid.CenterBottom) //bottom left
			};
		}

		/// <summary>
		/// check if the passed in collideable is colliding with anything
		/// </summary>
		/// <returns>return what it is colliding w ith</returns>
		public ICollides CollidingWith(ICollides self) {
			Node n = FetchContaining(self);
			
			//guard
			if (n == null)
				return null;

			Node[] adjacentNodes = n.parent.childNodes;

			foreach (Node adjacentNode in adjacentNodes) {
				foreach (ICollides other in adjacentNode.ICollidesList) {
					if (self != other && self.Team != other.Team && !adjacentNode.CheckedList.Contains(other)) {
						if (IsColliding(self.BoundingBox, other.BoundingBox)) {
							if (self is Ammo)
								CheckedList.Add(self);
							if (other is Ammo)
								CheckedList.Add(other);
							return other;
						}
					}
				}
			}
			
			return null;
		}

		/// <summary>
		/// Checks collision between line segments
		/// </summary>
		private bool IsColliding(List<LineSegment> bounds1, List<LineSegment> bounds2) {
			foreach(LineSegment l1 in bounds1) {
				foreach(LineSegment l2 in bounds2) {
					if (SwinGame.LineSegmentsIntersect(l1, l2))
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Register collidable to the right node
		/// </summary>
		public void Register(ICollides obj) {
			//guards
			if (obj == null)
				return;
			if (!SwinGame.RectOnScreen(grid))
				return;

			//end condition
			if (childNodes == null) {
				ICollidesList.Add(obj);
				return;
			}

			//find the node which contains the entiites position
			foreach(Node n in childNodes) {
				//only explore traverse relevant nodes
				if (n.ContainsPos(obj.RealPos)) {
					n.Register(obj);
				}
			}
		}

		/// <summary>
		/// remove the collideable entity from the tree
		/// </summary>
		/// <param name="obj">collideable object</param>
		public void Deregister(ICollides obj) {
			Node n = FetchContaining(obj);

			if (n != null)
				n.ICollidesList.Remove(obj);
		}

		/// <summary>
		/// clear the tree of all collideable entities
		/// </summary>
		public void Clear() {
			ICollidesList?.Clear();

			if (childNodes == null)
				return;

			foreach (Node n in childNodes) {
				n.Clear();
			}
		}

		/// <summary>
		/// Traverse the tree to fetch the node that contains the specified collideable object
		/// </summary>
		/// <param name="obj">collideable object</param>
		/// <returns>The containing node</returns>
		private Node FetchContaining(ICollides obj) {
			//guard
			if (obj == null)
				return null;

			//end condition (no more child nodes)
			if (childNodes == null) {
				if (ICollidesList.Contains(obj))
					return this;
				else return null;
			}
			else {
				//traverse
				foreach (Node n in childNodes) {
					if (n.ContainsPos(obj.RealPos))
						return n.FetchContaining(obj);
				}

				return null;
			}
		}

		/// <summary>
		/// Checks whether the specified point is within the node's area
		/// </summary>
		/// <param name="pos">An x,y point</param>
		/// <returns>true or false</returns>
		public bool ContainsPos(Point2D pos) {
			return pos.InRect(grid);
		}
	}
}
