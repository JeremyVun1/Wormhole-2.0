using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.Entities
{
	/// <summary>
	/// Ammo that locks onto and chases objects on other teams
	/// </summary>
	public class EmittingAmmo : Ammo
	{
		private IHandlesEntities entityHandler;
		private CooldownHandler primingTimer; //delay before we start turning or emitting
		private List<Component> emitters;

		public EmittingAmmo(string id, string filePath, Point2D refPos, Point2D offsetPos, Shape shape, List<Color> colors,
			int mass, int damage, float lifetime, float vel, float maxVel, float primingDelay, float turnRate,
			List<Component> emitters, BoundaryStrategy boundaryStrat, IHandlesEntities entHandler, Team team
		) : base(id, filePath, refPos, offsetPos, shape, colors, mass, damage, lifetime, vel, maxVel, turnRate, boundaryStrat, team)
		{
			primingTimer = new CooldownHandler(primingDelay);
			primingTimer.StartCooldown();
			this.emitters = emitters;

			entityHandler = entHandler;
		}

		public override void Update() {
			base.Update();
			
			HandleEmitters();
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

		public override void ForwardCommand() {
			if (primingTimer.IsOnCooldown())
				return;

			base.ForwardCommand();
		}

		/// <summary>
		/// initialise the emitting ammo
		/// </summary>
		/// <param name="pos">spawning position</param>
		/// <param name="dir">spawning direction</param>
		/// <param name="vel">spawning velocity</param>
		public override void Init(Point2D pos, Vector dir, Vector vel) {
			emitters?.TeleportTo(pos);
			base.Init(pos, dir, vel);
		}
	}
}
