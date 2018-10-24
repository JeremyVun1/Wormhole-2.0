using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// handles game camera behaviour
	/// </summary>
	public class CameraHandler
	{
		private Rectangle playArea;
		private Size2D<int> windowSize;
		private Point2D camOffset;
		private Rectangle chaseArea;
		private List<Rectangle> cornerAreas;
		private List<Rectangle> sideAreas;

		public Rectangle Viewport {
			get { return SwinGame.CreateRectangle(Camera.CameraPos(), SwinGame.ScreenWidth(), SwinGame.ScreenHeight()); }
		}

		private Entity activeEntity;

		private int rectLoc;
		private State state;
		private enum State { CORNER, SIDE, CHASE };

		public CameraHandler(Entity ent, Rectangle playArea) {
			activeEntity = ent;
			this.playArea = playArea;
			windowSize = new Size2D<int>(SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
			camOffset = SwinGame.PointAt(windowSize.W / 2, windowSize.H / 2);

			chaseArea = BuildChaseArea();
			cornerAreas = BuildCornerAreas();
			sideAreas = BuildSideAreas();
		}

		public void Update() {
			if (DebugMode.IsDebugging(Debugging.Camera)) {
				DebugArea(cornerAreas, SwinGame.RGBAColor(255, 0, 0, 80));
				DebugArea(sideAreas, SwinGame.RGBAColor(255, 0, 0, 50));
				DebugArea(chaseArea, SwinGame.RGBAColor(255, 0, 0, 20));
			}

			UpdateState();

			switch (state) {
				case State.CORNER:
					CornerCam();
					break;
				case State.SIDE:
					SideCam();
					break;
				case State.CHASE:
					ChaseCam();
					break;
			}
		}

		private void UpdateState() {
			if (IsEntityIn(cornerAreas))
				state = State.CORNER;
			else if (IsEntityIn(sideAreas))
				state = State.SIDE;
			else state = State.CHASE;
		}

		/// <summary>
		/// checks whether the entity is within the specified rectangle bound
		/// </summary>
		/// <param name="rects">a rectangle bound</param>
		private bool IsEntityIn(List<Rectangle> rects) {
			for(int i=0; i<4; ++i) {
				if (SwinGame.PointInRect(activeEntity.RealPos, rects[i])) {
					rectLoc = i;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Camera behaviour when the player is in the corner of the play area
		/// lock camera x and y from moving
		/// </summary>
		private void CornerCam() {
			switch (rectLoc) {
				case 0:
					Camera.MoveCameraTo(CenterOn(chaseArea.TopLeft));
					break;
				case 1:
					Camera.MoveCameraTo(CenterOn(chaseArea.TopRight));
					break;
				case 2:
					Camera.MoveCameraTo(CenterOn(chaseArea.BottomRight));
					break;
				case 3:
					Camera.MoveCameraTo(CenterOn(chaseArea.BottomLeft));
					break;
			}
		}

		/// <summary>
		/// Camera behaviour when the player is on the side of the play area
		/// clamp either x or y depending on which side the player is on
		/// </summary>
		private void SideCam() {
			switch (rectLoc) {
				case 0: // top - clamp y
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(activeEntity.RealPos.X, chaseArea.Top)));
					break;
				case 1: //right - clamp x
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(chaseArea.Right, activeEntity.RealPos.Y)));
					break;
				case 2: //bottom - clamp y
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(activeEntity.RealPos.X, chaseArea.Bottom)));
					break;
				case 3: //left - clamp x
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(chaseArea.Left, activeEntity.RealPos.Y)));
					break;
			}
		}

		/// <summary>
		/// Free chasing camera behaviour when the player is within the middle of the play area
		/// </summary>
		private void ChaseCam() {
			Camera.MoveCameraTo(CenterOn(activeEntity.RealPos));
		}

		/// <summary>
		/// Offset camera position so that it is centered on the target point
		/// </summary>
		/// <param name="target">point to center camera on</param>
		/// <returns>Offset camera position</returns>
		private Point2D CenterOn(Point2D target) {
			return target.Subtract(camOffset);
		}

		/// <summary>
		/// Define the area in which the camera will freely chase the player
		/// inset of the play area by the camera offset
		/// </summary>
		/// <returns>Rectangle</returns>
		private Rectangle BuildChaseArea() {
			Point2D TopLeft = playArea.TopLeft.Add(camOffset);
			Point2D BottomRight = playArea.BottomRight.Subtract(camOffset);
			return SwinGame.CreateRectangle(TopLeft, BottomRight);
		}

		/// <summary>
		/// Define the corner areas of the play area
		/// </summary>
		/// <returns>List of rectangles</returns>
		private List<Rectangle> BuildCornerAreas() {
			//now that we know the chase cam rectangle
			return new List<Rectangle>() {
				SwinGame.CreateRectangle(playArea.TopLeft, chaseArea.TopLeft),
				SwinGame.CreateRectangle(chaseArea.Right, playArea.Top, playArea.Right, chaseArea.Top),
				SwinGame.CreateRectangle(chaseArea.BottomRight, playArea.BottomRight),
				SwinGame.CreateRectangle(playArea.Left, chaseArea.Bottom, chaseArea.Left, playArea.Bottom)
			};
		}

		/// <summary>
		/// Build the side areas of the play area
		/// </summary>
		/// <returns>List of rectangles</returns>
		private List<Rectangle> BuildSideAreas() {
			return new List<Rectangle>() {
				SwinGame.CreateRectangle(cornerAreas[0].TopRight, chaseArea.TopRight), //top
				SwinGame.CreateRectangle(cornerAreas[1].BottomLeft, cornerAreas[2].TopRight), //right
				SwinGame.CreateRectangle(chaseArea.BottomLeft, cornerAreas[2].BottomLeft), //bottom
				SwinGame.CreateRectangle(cornerAreas[0].BottomLeft, chaseArea.BottomLeft) //left
			};
		}

		private void DebugArea(List<Rectangle> areas, Color clr) {
			foreach (Rectangle r in areas) {
				DebugArea(r, clr);
			}
		}

		private void DebugArea(Rectangle rect, Color clr) {
			SwinGame.FillRectangle(clr, rect);
		}
	}
}
