using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stateless;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class CommandButton : Button
	{
		ICommand command;

		public CommandButton(ICommand c, JToken btnJObj, JArray colors) : base(btnJObj, colors)
		{
			command = c;

			stateMachine.Configure(State.CLICKED)
				.OnEntry(() => command.Execute());
		}
	}
}
