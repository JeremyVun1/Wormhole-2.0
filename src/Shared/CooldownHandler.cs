using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace TaskForceUltra
{
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

		public void Update() {
			switch (stateMachine.State) {
				case State.COOLDOWN:
					if (timer.Ticks > threshhold)
						stateMachine.Fire(Trigger.TOGGLE);
					break;
			}
		}

		public void StartCooldown() {
			if (stateMachine.State == State.READY)
				stateMachine.Fire(Trigger.TOGGLE);
		}

		public bool OnCooldown() {
			return (stateMachine.State == State.COOLDOWN);
		}

		public void StartNewThreshhold(float ms) {
			threshhold = ms;
			stateMachine.Fire(Trigger.RESET);
			StartCooldown();
		}
	}
}
