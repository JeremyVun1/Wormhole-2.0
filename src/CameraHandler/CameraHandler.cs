using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace Wormhole
{
	public class CameraHandler
	{
		private Point2D camPos;
		private Point2D camOffset; //offset value to center camera on target
		private Rectangle camBounds; //boundary within play area that camera cannot go beyond
		private List<Rectangle> outOfBounds; //bounding boxes representing out of bounds;
		private Size2D<int> playSize;
		private Size2D<int> windSize;

		private IControllableShip activeShip;

		private enum State { CORNER, SIDE, CHASE }
		private enum Trigger { ZERO, ONE, TWO, THREE }
		StateMachine<State, Trigger> stateMachine;
		
		public CameraHandler(IControllableShip s, Size2D<int> playSize)
		{
			activeShip = s;
			this.playSize = playSize;
			windSize = new Size2D<int>(SwinGame.ScreenWidth(), SwinGame.ScreenHeight());

			camOffset = SwinGame.PointAt(-windSize.W / 2, -windSize.H / 2);
			camBounds = SwinGame.CreateRectangle(
				0 + windSize.W/2, //left
				0 + windSize.H/2, //top
				windSize.W - windSize.W/2, //right
				windSize.H - windSize.H/2 //bottom
			);

			//build the out of bound boxes for checking camera state;
			outOfBounds = new List<Rectangle>();
			BuildOutOfBoundsBoxes();

			stateMachine = new StateMachine<State, Trigger>(State.CHASE);
			ConfigureStateMachine();

			camPos = CenterOn(s.Pos);
		}

		public void SetActiveShip(IControllableShip s)
		{
			activeShip = s;
		}

		private void ConfigureStateMachine()
		{
			stateMachine.Configure(State.CORNER)
				.Permit(Trigger.ZERO, State.CHASE)
				.Permit(Trigger.ONE, State.SIDE)
				.Ignore(Trigger.TWO)
				.Ignore(Trigger.THREE);
			stateMachine.Configure(State.SIDE)
				.Permit(Trigger.ZERO, State.CHASE)
				.Ignore(Trigger.ONE)
				.Permit(Trigger.TWO, State.CORNER)
				.Ignore(Trigger.THREE);
			stateMachine.Configure(State.CHASE)
				.Ignore(Trigger.ZERO)
				.Permit(Trigger.ONE, State.SIDE)
				.Permit(Trigger.TWO, State.CORNER)
				.Ignore(Trigger.THREE);
		}

		public void Update()
		{
			HandleCameraState();

			switch(stateMachine.State)
			{
				case State.CORNER:
					//don't move the camera!!
					break;
				case State.SIDE:
					SideCam();
					break;
				case State.CHASE:
					ChaseCam();
					break;
			}

			Debug();
		}

		private void HandleCameraState()
		{
			stateMachine.Fire((Trigger)OutOfBoundCount());
		}

		private int OutOfBoundCount()
		{
			int result = 0;
			foreach(Rectangle r in outOfBounds)
			{
				if (activeShip.Pos.InRect(r))
					result++;
			}
			return result;
		}

		private void ChaseCam()
		{
			Camera.MoveCameraTo(CenterOn(activeShip.Pos));
		}

		private void SideCam()
		{
			//figure out which side the player is on (top, right, bottom, left)
			int i;
			for(i = 0; i < 4; ++i)
			{
				if (activeShip.Pos.InRect(outOfBounds[i]))
					break;
			}

			//lock camera to that side
			switch (i)
			{
				case 0: //top - lock Y
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(activeShip.Pos.X, camBounds.Top)));
					break;
				case 1: //right - lock X
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(camBounds.Right, activeShip.Pos.Y)));
					break;
				case 2: //bottom - Lock Y
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(activeShip.Pos.X, camBounds.Bottom)));
					break;
				case 3: //left - Lock X
					Camera.MoveCameraTo(CenterOn(SwinGame.PointAt(camBounds.Left, activeShip.Pos.Y)));
					break;
			}
		}

		private Point2D CenterOn(Point2D target)
		{
			return target.Add(camOffset);
		}

		private void BuildOutOfBoundsBoxes()
		{
			const int n = 4; //number of boxes (top, right, bottom, left)

			int[] x = new int[n] { 0, (int)camBounds.Right, 0, 0 };
			int[] y = new int[n] { 0, 0, (int)camBounds.Bottom, 0};
			int[] w = new int[n] { playSize.W, windSize.W / 2, playSize.W, windSize.W / 2 };
			int[] h = new int[n] { windSize.H / 2, playSize.H, windSize.H / 2, playSize.H };

			for (int i = 0; i < n; ++i)
			{
				Rectangle result = SwinGame.CreateRectangle(x[i], y[i], w[i], h[i]);
				outOfBounds.Add(result);
			}
		}

		private void Debug()
		{
			foreach(Rectangle r in outOfBounds)
			{
				SwinGame.FillRectangle(Color.Pink, r);
			}
			SwinGame.DrawRectangle(Color.Yellow, camBounds);
		}
	}
}
