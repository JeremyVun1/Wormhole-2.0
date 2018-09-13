using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class Star
	{
		private List<Color> colors;
		private int currColorIndex;
		private Point2D pos;
		private int size;
		private MinMax<int> sizeRange;

		//timing
		private MinMax<float> flareRate; //per second
		private MinMax<float> dimRate; //per second
		private float flareTrigger;
		private float dimTrigger;
		private Random rng;

		private Timer timer;

		private enum State { REST, DIMMING }
		private enum Trigger { FLARE, RESET }

		StateMachine<State, Trigger> stateMachine;

		public Star(List<Color> colors, JToken sizeMinMax, JToken dRateMinMax, JToken fRateMinMax, Size2D<int> playArea)
		{
			this.colors = colors;
			currColorIndex = 0;
			rng = new Random(Guid.NewGuid().GetHashCode());

			//Create star at random position
			int x = SwinGame.Rnd(playArea.W);
			int y = SwinGame.Rnd(playArea.H);
			sizeRange = new MinMax<int>(sizeMinMax.Value<int>("Min"), sizeMinMax.Value<int>("Max"));
			pos = SwinGame.PointAt(x, y);
			size = sizeRange.Min;

			//state machine
			 stateMachine = new StateMachine<State, Trigger>(State.REST);
			ConfigureStateMachine();

			//timing and rates
			flareRate = new MinMax<float>(fRateMinMax.Value<float>("Min"), fRateMinMax.Value<float>("Max"));
			dimRate = new MinMax<float>(dRateMinMax.Value<float>("Min"), dRateMinMax.Value<float>("Max"));
			flareTrigger = RandomTrigger(flareRate);
			dimTrigger = RandomTrigger(dimRate);

			timer = SwinGame.CreateTimer();
			timer.Start();

			//state with 15% stars in flared state
			flareTrigger -= 750;
		}

		public void ConfigureStateMachine()
		{
			stateMachine.Configure(State.REST)
				.OnEntry(() => Reset())
				.Permit(Trigger.FLARE, State.DIMMING)
				.Ignore(Trigger.RESET);
			stateMachine.Configure(State.DIMMING)
				.OnEntry(() => Flare())
				.Permit(Trigger.RESET, State.REST)
				.Ignore(Trigger.FLARE);
		}

		public void Draw()
		{
			//Log.Msg("star is drawing");
			SwinGame.FillRectangle(colors?[currColorIndex], pos.X, pos.Y, size, size);
		}

		public void Update()
		{
			switch(stateMachine.State)
			{
				case State.REST:
					if (flareTrigger < timer.Ticks)
						stateMachine.Fire(Trigger.FLARE);
					break;
				case State.DIMMING:
					Dim();
					break;
			}
		}

		private void Flare()
		{
			Reset();
			size = rng.Next(sizeRange.Min, sizeRange.Max + 1);
			ChangeColor();
		}

		private void Dim()
		{
			if (dimTrigger < timer.Ticks)
			{
				ChangeColor();
				DecrementSize();
				timer.Reset();
			}
		}

		private void ChangeColor()
		{
			currColorIndex = SwinGame.Rnd(colors.Count);
		}

		private void DecrementSize()
		{
			if (size > sizeRange.Min)
				size--;
			else stateMachine.Fire(Trigger.RESET);
		}

		private void Reset()
		{
			currColorIndex = 0;
			flareTrigger = RandomTrigger(flareRate);
			dimTrigger = RandomTrigger(dimRate);
			timer.Reset();
		}

		private float RandomTrigger(MinMax<float> minMax)
		{
			float result = rng.Next((int)(minMax.Min * 1000), (int)(minMax.Max * 1000));
			result = 1 / (result / 1000000);

			return result;
		}
	}
}
