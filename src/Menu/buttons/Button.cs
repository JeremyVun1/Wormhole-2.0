using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace Wormhole
{
	public abstract class Button
	{
		public string Label { get; set; }
		public Point2D Pos { get; set; }
		public Size2D<float> Size { get; set; }
		public BtnAction Action { get; set; }
		public string Payload { get; set; }
		public bool Selected { get; set; }
		public bool Clicked { get; private set; }

		private bool IsHovered {get; set; }
		private Color HoverColor { get; set; }
		private Color FontColor { get; set; }
		private Color FillColor
		{
			get
			{
				if (IsHovered)
					return HoverColor;
				return FillColor;
			}
			set { FillColor = value; }
		}
		private Color BorderColor { get; set; }

		private Rectangle btnShape
		{
			get { return SwinGame.CreateRectangle(Pos, Size.W, Size.H);	}
		}

		/*public Button(Color font, Color fill, Color border, ButtonHandler listener)
		{
			HoverColor = Color.Yellow;
			FontColor = font;
			FillColor = fill;
			BorderColor = border;
			Clicked = false;
		}*/

		public void Draw()
		{
			 //mock rectangle shape

			SwinGame.DrawText(Label, FontColor, FillColor, "label", FontAlignment.AlignCenter, btnShape); //button label
			SwinGame.FillRectangle(FillColor, btnShape); //fll
			SwinGame.DrawRectangle(BorderColor, btnShape); //outline
		}

		public void Update()
		{
			//update hover status
			if (ButtonHovered())
				IsHovered = true;
			else IsHovered = false;

			if (ButtonClicked())
				Clicked = true;
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
