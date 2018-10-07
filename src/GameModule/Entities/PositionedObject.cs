using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;


namespace TaskForceUltra.src.GameModule
{
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
	}
}
