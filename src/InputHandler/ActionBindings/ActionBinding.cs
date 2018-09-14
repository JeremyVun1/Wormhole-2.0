using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace Wormhole
{
	public class ActionBinding : IActionBinding
	{
		private Dictionary<ShipAction, KeyCode> bindings;

		public ActionBinding(Dictionary<ShipAction, KeyCode> bindings)
		{
			this.bindings = bindings;
		}

		public bool Forward() { return SwinGame.KeyDown(bindings[ShipAction.Forward]); }
		public bool Backward() { return SwinGame.KeyDown(bindings[ShipAction.Backward]); }
		public bool StrafeLeft() { return SwinGame.KeyDown(bindings[ShipAction.StrafeLeft]); }
		public bool StrafeRight() { return SwinGame.KeyDown(bindings[ShipAction.StrafeRight]); }
		public bool TurnLeft() { return SwinGame.KeyDown(bindings[ShipAction.TurnLeft]); }
		public bool TurnRight() { return SwinGame.KeyDown(bindings[ShipAction.TurnRight]); }
		public bool Shoot() { return SwinGame.KeyDown(bindings[ShipAction.Shoot]); }
		public bool ActivatePowerup() { return SwinGame.KeyDown(bindings[ShipAction.ActivatePowerup]); }
	}
}