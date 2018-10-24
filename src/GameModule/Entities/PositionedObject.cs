using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;


namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// base class manages the position of game objects
	/// </summary>
	public abstract class PositionedObject
	{
		protected Point2D refPos;
		protected Point2D offsetPos;
		public Point2D RealPos { get { return refPos.Add(offsetPos); } }

		public PositionedObject(Point2D refPos, Point2D offsetPos) {
			this.refPos = refPos;
			this.offsetPos = offsetPos;
		}
		
		public virtual void TeleportTo(Point2D target) {
			refPos = target;
		}

		/// <summary>
		/// Returns whether the object is on the screen or not
		/// </summary>
		protected bool IsOnScreen() {
			Rectangle cameraBox = SwinGame.CreateRectangle(Camera.CameraPos(), SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
			return SwinGame.PointInRect(RealPos, cameraBox);
		}
	}
}
