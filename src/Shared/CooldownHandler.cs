using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace TaskForceUltra
{
	/// <summary>
	/// Management of a swingame timer
	/// </summary>
	public class CooldownHandler
	{
		private Timer timer;
		private float threshhold;

		private enum State { COOLDOWN, READY };
		private enum Trigger { TOGGLE, RESET };
		private StateMachine<State, Trigger> stateMachine;

		public CooldownHandler(float ms) {
			threshhold = ms;

			stateMachine = new StateMachine<State, Trigger>(State.READY);
			timer = SwinGame.CreateTimer();

			ConfigureStateMachine();
		}

		private void ConfigureStateMachine() {
			stateMachine.Configure(State.READY)
				.OnEntry(() => timer.Stop())
				.Permit(Trigger.TOGGLE, State.COOLDOWN)
				.Ignore(Trigger.RESET);
			stateMachine.Configure(State.COOLDOWN)
				.OnEntry(() => timer.Start())
				.Permit(Trigger.TOGGLE, State.READY)
				.Permit(Trigger.RESET, State.READY);
		}

		private void Update() {
			switch (stateMachine.State) {
				case State.COOLDOWN:
					if (timer.Ticks > threshhold)
						stateMachine.Fire(Trigger.TOGGLE);
					break;
			}
		}

		/// <summary>
		/// Start the cooldown
		/// </summary>
		public void StartCooldown() {
			if (stateMachine.State == State.READY)
				stateMachine.Fire(Trigger.TOGGLE);
		}

		/// <summary>
		/// checks whether the timer is on cooldown or not
		/// </summary>
		/// <returns>true or false</returns>
		public bool IsOnCooldown() {
			Update();
			return (stateMachine.State == State.COOLDOWN);
		}

		/// <summary>
		/// Start the timer with a new cooldown threshhold
		/// </summary>
		/// <param name="ms">milliseconds</param>
		public void StartNewThreshhold(float ms) {
			threshhold = ms;
			stateMachine.Fire(Trigger.RESET);
			StartCooldown();
		}
	}
}
