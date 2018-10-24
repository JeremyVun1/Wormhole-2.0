using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace TaskForceUltra.src.MenuModule
{
	public class Button : MenuElement
	{
		private ICommand command;

		private enum State { HOVERED, CLICKED, REST }
		private enum Trigger { CLICK, HOVER, UNHOVER, RESET };
		private StateMachine<State, Trigger> stateMachine;

		public bool IsSelected {
			get {
				return stateMachine.State == State.CLICKED;
			}
		}

		public Button(string id, ICommand command, Rectangle bounds, Color hover, Color fill,
			Color border, Color font, string text
		) : base(id, bounds, hover, fill, border, font, text, "ButtonText", FontAlignment.AlignCenter)
		{
			this.command = command;
			stateMachine = new StateMachine<State, Trigger>(State.REST);
			ConfigureStateMachine();
		}

		public void Update() {
			if (ButtonHovered())
				stateMachine.Fire(Trigger.HOVER);
			else stateMachine.Fire(Trigger.UNHOVER);

			if (ButtonClicked())
				Click();
		}

		private void ConfigureStateMachine() {
			stateMachine.Configure(State.CLICKED)
				.OnEntry(() => Execute())
				.Permit(Trigger.CLICK, State.REST)
				.Permit(Trigger.RESET, State.REST)
				.Ignore(Trigger.HOVER)
				.Ignore(Trigger.UNHOVER);
			stateMachine.Configure(State.REST)
				.Permit(Trigger.CLICK, State.CLICKED)
				.Permit(Trigger.HOVER, State.HOVERED)
				.Ignore(Trigger.RESET)
				.Ignore(Trigger.UNHOVER);
			stateMachine.Configure(State.HOVERED)
				.Permit(Trigger.CLICK, State.CLICKED)
				.Permit(Trigger.UNHOVER, State.REST)
				.Permit(Trigger.RESET, State.REST)
				.Ignore(Trigger.HOVER);
		}

		private bool ButtonHovered() {
			return SwinGame.MousePosition().InRect(bounds);
		}

		private bool ButtonClicked() {
			return (ButtonHovered() && SwinGame.MouseClicked(MouseButton.LeftButton));
		}

		public override void Draw() {
			if (stateMachine.State != State.REST) {
				SwinGame.FillRectangle(hoverColor, bounds);
				SwinGame.DrawText(text, Color.Black, Color.Transparent, fontId, alignment, bounds);
				DrawOutline();
			}
			else base.Draw();
		}

		public void Reset() {
			stateMachine.Fire(Trigger.RESET);
		}

		public virtual void Click() {
			stateMachine.Fire(Trigger.CLICK);
		}

		protected virtual void Execute() {
			command.Execute();
		}
	}
}
