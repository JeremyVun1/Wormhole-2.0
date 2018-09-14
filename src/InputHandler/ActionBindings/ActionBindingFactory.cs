using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class ActionBindingFactory
	{
		private string bindingsDir;
		private Dictionary<ControllerType, ActionBinding> bindingRegistry;

		public ActionBindingFactory(string bindingsDir)
		{
			this.bindingsDir = bindingsDir;
			bindingRegistry = new Dictionary<ControllerType, ActionBinding>();

			RegisterBindings();
		}

		public void RegisterBindings()
		{
			string[] fileList = Directory.GetFiles(bindingsDir);

			foreach (string file in fileList)
			{
				Dictionary<ShipAction, KeyCode> binding = new Dictionary<ShipAction, KeyCode>();

				JObject obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(file));

				//parse controller type
				ControllerType controller = (ControllerType)Enum.Parse(typeof(ControllerType), obj.Value<string>("id"));

				//parse bindings
				foreach (JProperty prop in obj.GetValue("Bindings").OfType<JProperty>())
				{
					Enum.TryParse(prop.Name, out ShipAction key);
					Enum.TryParse((string)prop.Value, out KeyCode value);

					binding.Add(key, value);
				}

				//add <controllertype><actionbinding> entry to the registry
				bindingRegistry.Add(controller, new ActionBinding(binding));
			}
		}

		public IActionBinding Fetch(ControllerType type)
		{
			if (bindingRegistry.ContainsKey(type))
				return bindingRegistry[type];
			return null;
		}
	}
}
