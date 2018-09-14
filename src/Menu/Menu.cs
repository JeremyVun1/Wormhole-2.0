using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SwinGameSDK;

namespace Wormhole
{
	public class Menu : IModule
	{
		[JsonProperty]
		public string MenuId { get; private set; }
		[JsonProperty]
		private string Title;
		[JsonIgnore]
		private List<IMenuElement> elements { get; set; }
		[JsonIgnore]
		private Level scene;

		public Menu(string json, List<IMenuElement> b, List<IMenuElement> t, Level s)
		{
			JsonConvert.PopulateObject(json, this);
			elements = new List<IMenuElement>();
			elements.AddRange(t);
			elements.AddRange(b);

			scene = s;
		}

		//transition
		public bool Ended { get; private set; }

		//player progress
		public Player PlayerProgress { get; }

		public void Update()
		{
			//update menu scene
			scene.Update();

			//update menu elements
			foreach (IMenuElement e in elements)
			{
				e.Update();
			}
		}

		public void Draw()
		{
			//draw menu scene
			scene.Draw();

			//draw elements
			foreach (IMenuElement e in elements)
			{
				e.Draw();
			}

			//draw title
			Rectangle window = SwinGame.CreateRectangle(0, 0, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
			SwinGame.DrawText(Title, Color.White, Color.Transparent, "MenuTitle", FontAlignment.AlignCenter, window);
		}

		public void ResetButtons()
		{
			foreach(CommandButton b in elements.OfType<CommandButton>())
			{
				b.Reset();
			}
		}
	}
}
