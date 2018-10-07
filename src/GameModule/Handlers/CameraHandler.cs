using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace TaskForceUltra.src.GameModule
{
	public class CameraHandler
	{
		private Rectangle playArea;
		private Size2D<int> windowSize;
		private Point2D camOffset;
		private Rectangle chaseArea;
		private List<Rectangle> cornerAreas;
		private List<Rectangle> sideAreas;

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

		private void DebugArea(List<Rectangle> areas, Color clr) {
			foreach(Rectangle r in areas) {
				SwinGame.FillRectangle(clr, r);
			}
		}

		public void Update() {
			//DebugArea(cornerAreas, Color.Red);
			//DebugArea(sideAreas, Color.Blue);
			
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
			if (EntityIn(cornerAreas))
				state = State.CORNER;
			else if (EntityIn(sideAreas))
				state = State.SIDE;
			else state = State.CHASE;
		}

		private bool EntityIn(List<Rectangle> rects) {
			for(int i=0; i<4; ++i) {
				if (SwinGame.PointInRect(activeEntity.RealPos, rects[i])) {
					rectLoc = i;
					return true;
				}
			}
			return false;
		}

		private void CornerCam() {
			//clamp cam x and y to chaseArea bounds
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

		private void SideCam() {
			//clamp x or y based on which side we are on
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

		private void ChaseCam() {
			Camera.MoveCameraTo(CenterOn(activeEntity.RealPos));
		}

		private Point2D CenterOn(Point2D target) {
			return target.Subtract(camOffset);
		}

		private Rectangle BuildChaseArea() {
			//inset of playarea by camOffset
			Point2D TopLeft = playArea.TopLeft.Add(camOffset);
			Point2D BottomRight = playArea.BottomRight.Subtract(camOffset);
			return SwinGame.CreateRectangle(TopLeft, BottomRight);
		}

		private List<Rectangle> BuildCornerAreas() {
			//now that we know the chase cam rectangle
			return new List<Rectangle>() {
				SwinGame.CreateRectangle(playArea.TopLeft, chaseArea.TopLeft),
				SwinGame.CreateRectangle(chaseArea.Right, playArea.Top, playArea.Right, chaseArea.Top),
				SwinGame.CreateRectangle(chaseArea.BottomRight, playArea.BottomRight),
				SwinGame.CreateRectangle(playArea.Left, chaseArea.Bottom, chaseArea.Left, playArea.Bottom)
			};
		}

		private List<Rectangle> BuildSideAreas() {
			//now that we know chase area and corner areas
			return new List<Rectangle>() {
				SwinGame.CreateRectangle(cornerAreas[0].TopRight, chaseArea.TopRight), //top
				SwinGame.CreateRectangle(cornerAreas[1].BottomLeft, cornerAreas[2].TopRight), //right
				SwinGame.CreateRectangle(chaseArea.BottomLeft, cornerAreas[2].BottomLeft), //bottom
				SwinGame.CreateRectangle(cornerAreas[0].BottomLeft, chaseArea.BottomLeft) //left
			};
		}
	}
}
