using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.MenuModule
{
	/// <summary>
	/// a button without sticky clicking behaviour
	/// </summary>
	public class NonStickButton : Button
	{
		public NonStickButton(string id, ICommand command, Rectangle bounds, Color hover,
			Color fill, Color border, Color font, string text
		) : base(id, command, bounds, hover, fill, border, font, text) { }

		protected override void Execute() {
			base.Execute();
			Reset();
		}
	}
}
