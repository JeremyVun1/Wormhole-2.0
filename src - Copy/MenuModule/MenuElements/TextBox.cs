using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// textbox for holding text
	/// </summary>
	public class TextBox : MenuElement
	{
		public TextBox(string id, Rectangle bounds, Color hover, Color fill,
			Color border, Color font, string text
		) : base(id, bounds, hover, Color.Transparent, border, font, text, "MenuText", FontAlignment.AlignLeft) { }

		protected override void DrawOutline() { }
	}
}
