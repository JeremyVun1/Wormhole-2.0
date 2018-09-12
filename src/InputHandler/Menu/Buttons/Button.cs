using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stateless;

namespace Wormhole
{
	public abstract class Button
	{
		private string Label { get; set; }
		private Point2D Pos { get; set; } //relative position
		private Size2D<float> Size { get; set; }
		private string Payload { get; set; }

		private bool Clicked { get; set; }
		private bool Hovered {get; set; }

		private Color HoverColor { get; set; }
		private Color FontColor { get; set; }
		private Color FillColor { get; set; }
		private Color BorderColor { get; set; }
		private Rectangle btnShape
		{
			get
			{
				Point2D absolutePos = SwinGame.PointAt(Pos.X * SwinGame.ScreenWidth(), Pos.Y * SwinGame.ScreenHeight());
				Size2D<int> absoluteSize = new Size2D<int>((int)Size.W * SwinGame.ScreenWidth(), (int)Size.H * SwinGame.ScreenHeight());
				return SwinGame.CreateRectangle(absolutePos, absoluteSize.W, absoluteSize.H);
			}
		}

		//state machine
		protected enum State { HOVERED, CLICKED, REST }
		protected enum Trigger { CLICK, HOVER, UNHOVER };
		protected StateMachine<State, Trigger> stateMachine;

		public Button(dynamic btnJObj, JObject colors)
		{
			Clicked = false;
			stateMachine = new StateMachine<State, Trigger>(State.REST);
			ConfigureStateMachine();

			//populate attributes
			Label = btnJObj.Label;
			Pos = btnJObj.Pos.ToObject<Point2D>();
			Size = btnJObj.Size.ToObject<Size2D<float>>();
			Payload = btnJObj.Payload;
			LoadColors(colors, colors.Count);
		}

		private Color GetRGBColor(JObject color)
		{
			return null;
			int r, g, b;
			Color result;

			//get rgb values from color object and return swingame color object
			foreach(var p in color)
			{
				switch(p.Key.ToString().ToLower())
				{
					case "r":
						//r = p;
						break;
					case "g":
						//g = p;
						break;
					case "b":
						//b = p;
						break;
				}
			}
		}

		private void LoadColors(JObject colors, int count)
		{
			foreach(var color in colors)
			{
				switch(color.Key.ToString().ToLower())
				{
					case "fontcolor":
						Console.WriteLine(color.Value);
						//FontColor = GetRGBColor(color.Value);
						break;
					case "fillcolor":
						break;
					case "bordercolor":
						break;
					case "hovercolor":
						break;
				}
			}

			//HoverColor = SwinGame.RGBColor((byte)colors[0].hover[0], (byte)hover[1], (byte)hover[2]);
			
			//FillColor = SwinGame.RGBColor((byte)fill[0], (byte)fill[1], (byte)fill[2]);
			//BorderColor = SwinGame.RGBColor((byte)border[0], (byte)border[1], (byte)border[2]);
		}

		public void Draw()
		{
			//label
			SwinGame.DrawText(Label, FontColor, FillColor, "label", FontAlignment.AlignCenter, btnShape);

			//fill
			if (stateMachine.State != State.REST)
				SwinGame.FillRectangle(HoverColor, btnShape);
			else SwinGame.FillRectangle(FillColor, btnShape);

			//outline
			SwinGame.DrawRectangle(BorderColor, btnShape);
		}

		public void Update()
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
				.Permit(Trigger.CLICK, State.REST);
			stateMachine.Configure(State.REST)
				.Permit(Trigger.CLICK, State.CLICKED)
				.Permit(Trigger.HOVER, State.HOVERED);
			stateMachine.Configure(State.HOVERED)
				.Permit(Trigger.CLICK, State.CLICKED)
				.Permit(Trigger.UNHOVER, State.REST);
		}

		private bool ButtonHovered()
		{
			return SwinGame.MousePosition().InRect(btnShape);
		}

		private bool ButtonClicked()
		{
			return (ButtonHovered() && SwinGame.MouseClicked(MouseButton.LeftButton));
		}
	}
}
