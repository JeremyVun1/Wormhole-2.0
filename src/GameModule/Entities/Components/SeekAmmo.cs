using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.AI;

namespace TaskForceUltra.src.GameModule.Entities
{
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

		public SeekAmmo(string id, string filePath, Point2D refPos, Point2D offsetPos,
			Shape shape, List<Color> colors, int mass, int damage, float lifetime, float vel, float maxVel,
			float turnRate, List<Component> emitters, BoundaryStrategy boundaryStrat, IHandlesEntities entHandler, Team team
		) : base(id, filePath, refPos, offsetPos, shape, colors, mass, damage, lifetime, vel, turnRate, boundaryStrat, team)
		{
			entityHandler = entHandler;
			MaxVel = maxVel;
			primingTimer = new CooldownHandler(1000);
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
			if (thrusting && !primingTimer.OnCooldown()) {
				foreach (Emitter e in emitters) {
					e.Update();
					e.Activate();
					e.TeleportTo(RealPos);
				}
				thrusting = false;
			}
		}

		public override void Init(Point2D pos, Vector dir, Vector vel) {
			emitters.TeleportTo(pos);
			base.Init(pos, dir, vel);
			thrustForce = accel;
		}

		public void TurnTo(Vector targetDir, float turnStrength = 1) {
			if (primingTimer.OnCooldown())
				return;

			double desiredTheta = Dir.AngleTo(targetDir) * Math.PI / 180;

			theta += turnRate * turnStrength * Math.PI / 180;
			theta *= desiredTheta.GetSign();

			if (theta <= 0) {
				theta = theta.Clamp(desiredTheta, 0);
			}
			else {
				theta = theta.Clamp(0, desiredTheta);
			}
		}

		public void Fire() { }
	}
}
