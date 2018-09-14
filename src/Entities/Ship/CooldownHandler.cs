using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace Wormhole
{
	public class CooldownHandler
	{
		private Timer timer;
		private float threshhold;

		private enum State { COOLDOWN, READY };
		private enum Trigger { TOGGLE };
		private StateMachine<State, Trigger> stateMachine;

		public CooldownHandler(float c)
		{
			threshhold = c;

			stateMachine = new StateMachine<State, Trigger>(State.READY);
			timer = SwinGame.CreateTimer();

			ConfigureStateMachine();
		}

		private void ConfigureStateMachine()
		{
			stateMachine.Configure(State.READY)
				.OnEntry(() => timer.Stop())
				.Permit(Trigger.TOGGLE, State.COOLDOWN);
			stateMachine.Configure(State.COOLDOWN)
				.OnEntry(() => timer.Start())
				.Permit(Trigger.TOGGLE, State.READY);
		}

		public void Update()
		{
			switch(stateMachine.State)
			{
				case State.COOLDOWN:
					if (timer.Ticks > threshhold)
						stateMachine.Fire(Trigger.TOGGLE);
					break;
			}
		}

		public void StartCooldown()
		{
			if (stateMachine.State == State.READY)
				stateMachine.Fire(Trigger.TOGGLE);
		}

		public bool OnCooldown()
		{
			return (stateMachine.State == State.COOLDOWN);
		}
	}
}
