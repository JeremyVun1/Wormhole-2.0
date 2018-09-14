using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class TextBoxFactory : MenuElementFactory
	{
		public override IMenuElement Create(JToken txtBox, JArray colors)
		{
			return new TextBox(txtBox, colors);
		}
	}
}
