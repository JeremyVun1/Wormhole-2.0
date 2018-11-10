using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.AI;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Ammo that locks onto and chases objects on other teams
	/// </summary>
	public class SeekAmmo : Ammo, IAIEntity
	{
		public float MaxVel {
			get { return maxVel; }
			set { maxVel = value; }
		}
		private float accel;
		private IHandlesEntities entityHandler;
		private AIStrategy aiStrat;
		public AIStrategy AIStrat { set { aiStrat = value; } }
		private CooldownHandler primingTimer; //delay before we start seeking behaviour
		private List<Component> emitters;

		public SeekAmmo(string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape, List<Color> colors,
			int mass, int damage, float lifetime, float vel, float maxVel, float primingDelay, float turnRate,
			List<Component> emitters, BoundaryStrategy boundaryStrat, IHandlesEntities entHandler, Team team
		) : base(id, filePath, refPos, offsetPos, shape, colors, mass, damage, lifetime, vel, turnRate, boundaryStrat, team)
		{
			entityHandler = entHandler;
			MaxVel = maxVel;
			primingTimer = new CooldownHandler(primingDelay);
			primingTimer.StartCooldown();
			this.emitters = emitters;
			accel = vel;
		}

		public override void Update() {
			if (sleep)
				return;

			aiStrat.Update();
			HandleEmitters();
			base.Update();
		}

		private void HandleEmitters() {
			if (emitters == null)
				return;

			if (thrusting && !primingTimer.IsOnCooldown()) {
				foreach (Emitter e in emitters) {
					e.Update();
					e.Activate();
					e.TeleportTo(RealPos);
				}
				thrusting = false;
			}
		}

		/// <summary>
		/// initialise the seeking ammo
		/// </summary>
		/// <param name="pos">spawning position</param>
		/// <param name="dir">spawning direction</param>
		/// <param name="vel">spawning velocity</param>
		public override void Init(Point2D pos, Vector dir, Vector vel) {
			emitters?.TeleportTo(pos);
			base.Init(pos, dir, vel);
			thrustForce = accel;
		}

		public void ForwardCommand() {
			if (primingTimer.IsOnCooldown())
				return;

			Thrust(Dir.UnitVector);
		}

		public void TurnRightCommand() {
			theta += turnRate * Math.PI / 180;
		}

		public void TurnLeftCommand() {
			theta -= turnRate * Math.PI / 180;
		}

		public void Fire() { }

		public void BackwardCommand() {}

		public void StrafeLeftCommand() {}

		public void StrafeRightCommand() {}

		public void ShootCommand() {}

		public void ActivatePowerupCommand() {}
	}
}
