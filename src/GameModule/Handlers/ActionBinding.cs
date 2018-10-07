using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra.src.GameModule
{
	public class ActionBinding : IActionBinding
	{
		private Dictionary<ShipAction, KeyCode> bindings;

		public ActionBinding(Dictionary<ShipAction, KeyCode> bindings) {
			this.bindings = bindings;
		}

		//check action bindings to see if player wants to perform a particular ship action
		public bool Forward() {
			return SwinGame.KeyDown(bindings[ShipAction.Forward]);
		}
		public bool Backward() {
			return SwinGame.KeyDown(bindings[ShipAction.Backward]);

		}
		public bool StrafeLeft() {
			return SwinGame.KeyDown(bindings[ShipAction.StrafeLeft]);
		}
		public bool StrafeRight() {
			return SwinGame.KeyDown(bindings[ShipAction.StrafeRight]);
		}
		public bool TurnLeft() {
			return SwinGame.KeyDown(bindings[ShipAction.TurnLeft]);
		}
		public bool TurnRight() {
			return SwinGame.KeyDown(bindings[ShipAction.TurnRight]);
		}
		public bool Shoot() {
			return SwinGame.KeyDown(bindings[ShipAction.Shoot]);
		}
		public bool ActivatePowerup() {
			return SwinGame.KeyTyped(bindings[ShipAction.ActivatePowerup]);
		}
	}

	/// <summary>
	/// Action Binding Factory
	/// </summary>
	public class ActionBindingFactory
	{
		private string dirPath;

		public ActionBindingFactory(string dirPath) {
			this.dirPath = dirPath;
		}

		public IActionBinding Create(ControllerType controller) {
			switch(controller) {
				case ControllerType.Player1:
					return ReadActionBinding("\\Player1.json");
				case ControllerType.Player2:
					return ReadActionBinding("\\Player2.json");
				case ControllerType.Player3:
					return ReadActionBinding("\\Player3.json");
				case ControllerType.Player4:
					return ReadActionBinding("\\Player4.json");
				default:
					return ReadActionBinding("\\Player1.json");
			}
		}

		private IActionBinding ReadActionBinding(string fileName) {
			string filePath = dirPath + fileName;
			if (!File.Exists(filePath))
				return null;

			var result = new Dictionary<ShipAction, KeyCode>();
			JObject bindingsObj = Util.Deserialize(filePath);

			//iterate through binding settings in the json object
			foreach (JProperty binding in bindingsObj.GetValue("Bindings").OfType<JProperty>()) {
				Enum.TryParse(binding.Name, out ShipAction key);
				Enum.TryParse((string)binding.Value, out KeyCode value);

				result.Add(key, value);
			}

			return new ActionBinding(result);
		}
	}
}
