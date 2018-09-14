using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class CommandButtonFactory : MenuElementFactory
	{
		MenuCommandFactory commandFac;

		public CommandButtonFactory(MenuModule m)
		{
			commandFac = new MenuCommandFactory(m);
		}

		public override IMenuElement Create(JToken btn, JArray colors)
		{
			//create command and inject into button
			ICommand command = commandFac.Create(btn.Value<string>("Action"), btn.Value<string>("Payload"));
			return new CommandButton(command, btn, colors);
		}
	}
}
