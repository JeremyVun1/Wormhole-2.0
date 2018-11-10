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
	public class EmittingAmmo : Ammo
	{
		private IHandlesEntities entityHandler;
		private CooldownHandler primingTimer; //delay before we start seeking behaviour
		private List<Component> emitters;

		public EmittingAmmo(string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape, List<Color> colors,
			int mass, int damage, float lifetime, float vel, float maxVel, float primingDelay, float turnRate,
			List<Component> emitters, BoundaryStrategy boundaryStrat, IHandlesEntities entHandler, Team team
		) : base(id, filePath, refPos, offsetPos, shape, colors, mass, damage, lifetime, vel, maxVel, turnRate, boundaryStrat, team)
		{
			entityHandler = entHandler;			
			primingTimer = new CooldownHandler(primingDelay);
			primingTimer.StartCooldown();
			this.emitters = emitters;
		}

		public override void Update() {
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
		}

		public override void TurnTo(Vector targetDir, float turnStrength = 1) {
			if (primingTimer.IsOnCooldown())
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
	}
}
