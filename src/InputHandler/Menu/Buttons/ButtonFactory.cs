using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class ButtonFactory
	{
		public Button Create(dynamic btnJObj, JObject colors)
		{
			BtnAction type = (BtnAction)Enum.Parse(typeof(BtnAction), (string)btnJObj.Action);

			//MenuCommandFactory commandFac = new MenuCommandFactory();
			//ICommand commandObj = commandFac.Create(btnJObj.Action.ToString(), btnJObj.Payload.ToString());

			//return new MenuButton(commandObj, btnJObj, colors);
			return null;
		}
	}
}
