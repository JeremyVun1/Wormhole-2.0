using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Something that moves around
	/// Handles movement logic
	/// </summary>
	public abstract class Mover : Entity, IControllable
	{
		public Vector Vel { get; protected set; }

		public double theta; //radians
		public Vector Dir { get; protected set; }

		protected BoundaryStrategy boundaryStrat { get; private set; }

		private bool isOptimisedUpdate; // if true, do not update the object if it is off screen

		public Mover(
			string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape,
			List<Color> colors, int health, Vector vel, Vector dir, BoundaryStrategy boundaryStrat,
			Team team, bool optimiseMe = false
		) : base(id, filePath, refPos, offsetPos, shape, colors, health, team)
		{
			this.boundaryStrat = boundaryStrat;
			Vel = vel;
			Dir = dir;
			theta = 0;

			isOptimisedUpdate = optimiseMe;
		}

		public override void Update() {
			if (isOptimisedUpdate && !IsOnScreen())
				return;

			base.Update();
			Move();
			Rotate();
			boundaryStrat?.Run(this);
		}

		/// <summary>
		/// Automatically move based on whatever the current velocity is
		/// </summary>
		protected void Move() {
			refPos += Vel;
			Shape?.Move(Vel);
		}

		/// <summary>
		/// Automatically turn based on whatever theta has been set
		/// </summary>
		protected void Rotate() {
			Dir = Dir.Rotate(theta);
			Shape?.Rotate(theta);
			offsetPos = offsetPos.Rotate(SwinGame.PointAt(0,0), theta);
			theta = 0;
		}

		protected override void Debug() {
			SwinGame.DrawLine(Color.Blue, SwinGame.LineFromVector(Vel));
			base.Debug();
		}

		/// <summary>
		/// checks if the difference between the target direction and the current direction is small enough to start thrusting forward
		/// </summary>
		public bool ShouldThrust(Vector targetDir) {
			return Math.Abs(Dir.AngleTo(targetDir)) < 45;
		}
	}
}
