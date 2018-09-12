using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json.Linq;
using Stateless;

namespace Wormhole
{
	public abstract class Button : MenuElement
	{
		private string label;
		private string payload;

		//state machine
		protected enum State { HOVERED, CLICKED, REST }
		protected enum Trigger { CLICK, HOVER, UNHOVER };
		protected StateMachine<State, Trigger> stateMachine;

		public Button(JToken btnJObj, JArray colorsObj)
		{
			stateMachine = new StateMachine<State, Trigger>(State.REST);
			ConfigureStateMachine();

			//populate attributes
			label = btnJObj.Value<string>("Label");
			payload = btnJObj.Value<string>("Payload");
			LoadColors(colorsObj);

			Point2D relPos = btnJObj["Pos"].ToObject<Point2D>();
			Size2D<float> relSize = btnJObj["Size"].ToObject<Size2D<float>>();
			boundingBox = BuildBoundingBox(relPos, relSize);
		}

		public override void Draw()
		{
			//fill
			if (stateMachine.State != State.REST)
				SwinGame.FillRectangle(hoverColor, boundingBox);
			else SwinGame.FillRectangle(fillColor, boundingBox);

			//outline
			SwinGame.DrawRectangle(borderColor, boundingBox);

			//label
			SwinGame.DrawText(label, Color.Aqua, Color.Transparent, "ButtonText", FontAlignment.AlignCenter, boundingBox);
		}

		public override void Update()
		{
			//update hover status
			if (ButtonHovered())
				stateMachine.Fire(Trigger.HOVER);
			else stateMachine.Fire(Trigger.UNHOVER);

			if (ButtonClicked())
				stateMachine.Fire(Trigger.CLICK);
		}

		private void ConfigureStateMachine()
		{
			stateMachine.Configure(State.CLICKED)
				.Permit(Trigger.CLICK, State.REST)
				.Ignore(Trigger.HOVER)
				.Ignore(Trigger.UNHOVER);
			stateMachine.Configure(State.REST)
				.Permit(Trigger.CLICK, State.CLICKED)
				.Permit(Trigger.HOVER, State.HOVERED)
				.Ignore(Trigger.UNHOVER);
			stateMachine.Configure(State.HOVERED)
				.Permit(Trigger.CLICK, State.CLICKED)
				.Permit(Trigger.UNHOVER, State.REST)
				.Ignore(Trigger.HOVER);
		}

		private bool ButtonHovered()
		{
			return SwinGame.MousePosition().InRect(boundingBox);
		}

		private bool ButtonClicked()
		{
			return (ButtonHovered() && SwinGame.MouseClicked(MouseButton.LeftButton));
		}
	}
}
