using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class MenuButton : Button
	{
		ICommand command;

		public MenuButton(ICommand c, JObject btnJObj, JObject colors) : base(btnJObj, colors)
		{
			command = c;

			stateMachine.Configure(State.CLICKED)
				.OnEntry(() => command.Execute());
		}
	}
}
