using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wormhole
{
	public class Menu : IModule
	{
		[JsonProperty]
		public string MenuId { get; private set; }
		[JsonProperty]
		private string Title;
		[JsonProperty]
		private int FontSize;
		[JsonIgnore]
		private List<TextBox> TextBoxes { get; set; }
		[JsonIgnore]
		public List<MenuButton> Buttons { get; set; }

		public Menu(string json, List<MenuButton> b, List<TextBox> t)
		{
			JsonConvert.PopulateObject(json, this);
			Buttons = b;
			TextBoxes = t;
		}

		//transition
		public bool Ended { get; private set; }

		//player progress
		public Player PlayerProgress { get; }

		public void Update()
		{
		}

		/*private void ActivateButton(BtnAction action, string payload)
		{
			
		}*/

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
