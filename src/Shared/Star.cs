using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaskForceUltra
{
	public class Star
	{
		private List<Color> colors;
		private int currColorIndex;
		private Point2D pos;
		private int size;
		private MinMax<int> sizeRange;

		//timing
		private Random rng;
		private MinMax<float> flareRate; //per second
		private MinMax<float> dimRate; //per second
		private float flareTrigger;
		private float dimTrigger;
		private CooldownHandler cdHandler;

		//state
		private enum State { REST, DIMMING }
		private enum Trigger { FLARE, RESET }
		private StateMachine<State, Trigger> stateMachine;

		public Star(List<Color> colors, MinMax<int> sizeMinMax, MinMax<float> dimRateMinMax,MinMax<float> flareRateMinMax, Rectangle playArea) {
			this.colors = colors;
			currColorIndex = 0;

			//Create star at random position
			rng = new Random(Guid.NewGuid().GetHashCode());
			pos = Util.RandomPointInRect(playArea);
			sizeRange = sizeMinMax;
			size = sizeRange.Min;

			//timing and rates
			flareRate = flareRateMinMax;
			dimRate = dimRateMinMax;
			flareTrigger = 1 / (Util.RandomInRange(flareRate) / 1000);
			dimTrigger = 1 / (Util.RandomInRange(dimRate) / 1000);
			flareTrigger -= 1000;
			cdHandler = new CooldownHandler(flareTrigger);

			//state machine
			stateMachine = new StateMachine<State, Trigger>(State.REST);
			ConfigureStateMachine();

			//start in random state
			stateMachine.Fire((Trigger)Util.Rand(2));

			if (stateMachine.State == State.REST)
				cdHandler = new CooldownHandler(flareTrigger);
			else cdHandler = new CooldownHandler(dimTrigger);

			cdHandler.StartCooldown();
		}

		private void ConfigureStateMachine() {
			stateMachine.Configure(State.REST)
				.OnEntry(() => Reset())
				.Permit(Trigger.FLARE, State.DIMMING)
				.Ignore(Trigger.RESET);
			stateMachine.Configure(State.DIMMING)
				.OnEntry(() => Flare())
				.Permit(Trigger.RESET, State.REST)
				.Ignore(Trigger.FLARE);
		}

		public void Draw() {
			SwinGame.FillRectangle(colors?[currColorIndex], pos.X, pos.Y, size, size);
		}

		public void Update() {
			switch (stateMachine.State) {
				case State.REST:
					if (!cdHandler.OnCooldown())
						stateMachine.Fire(Trigger.FLARE);
					break;
				case State.DIMMING:
					Dim();
					break;
			}
		}

		private void Flare() {
			cdHandler.StartNewThreshhold(dimTrigger);
			size = rng.Next(sizeRange.Min, sizeRange.Max + 1);
			ChangeColor();
		}

		private void Dim() {
			if (!cdHandler.OnCooldown()) {
				ChangeColor();
				DecrementSize();
			}
		}

		private void ChangeColor() {
			currColorIndex = SwinGame.Rnd(colors.Count);
		}

		private void DecrementSize() {
			if (size > sizeRange.Min)
				size--;
			else stateMachine.Fire(Trigger.RESET);
		}

		private void Reset() {
			currColorIndex = 0;
			flareTrigger = 1 / (Util.RandomInRange(flareRate) / 1000);
			dimTrigger = 1 / (Util.RandomInRange(dimRate) / 1000);
			cdHandler.StartNewThreshhold(flareTrigger);
		}
	}

	/// <summary>
	/// Star Factory
	/// </summary>
	public class StarFactory
	{
		public Star Create(MinMax<int> sizeRange, MinMax<float> dimRange,
			MinMax<float> flareRange, List<Color> starColors, Rectangle playArea) {
			return new Star(starColors, sizeRange, dimRange, flareRange, playArea);
		}

		public List<Star> CreateList(int count, MinMax<int> sizeRange, MinMax<float> dimRange,
			MinMax<float> flareRange, List<Color> starColors, Rectangle playArea) {
			List<Star> result = new List<Star>();

			for (int i = 0; i < count; ++i) {
				result.Add(Create(sizeRange, dimRange, flareRange, starColors, playArea));
			}
			return result;
		}
	}
}
