using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class Menu : IModule
	{
		public string MenuID { get; set; }
		public string Title { get; set; }
		public RGB FontColor { get; set; }
		public RGB FillColor { get; set; }
		public RGB BorderColor { get; set; }
		public int Size { get; set; }
		public List<TextBox> TextBoxes { get; set; }
		public List<NavButton> Buttons { get; set; }

		//transition
		public bool Ended { get; private set; }

		//player progress
		public Player PlayerProgress { get; }

		public void Update()
		{
			foreach(Button b in Buttons)
			{
				if (b.Clicked)
					ActivateButton(b.Action, b.Payload);
			}
		}

		private void ActivateButton(BtnAction action, string payload)
		{
			
		}

		public void Draw()
		{
			foreach (TextBox t in TextBoxes)
			{
				t.Draw();
			}
			foreach(Button b in Buttons)
			{
				b.Draw();
			}
		}
	}
}
