using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra
{
	/// <summary>
	/// handles debugging state
	/// </summary>
	public static class DebugMode
	{
		private static bool ship = false;
		private static bool ammo = false;
		private static bool particle = false;
		private static bool level = false;
		private static bool component = false;
		private static bool nodes = false;

		/// <summary>
		/// checks whether the type is being debugged
		/// </summary>
		/// <param name="type">debugging enum</param>
		/// <returns></returns>
		public static bool IsDebugging(Debugging type) {
			switch (type) {
				case Debugging.Ship:
					return ship;
				case Debugging.Ammo:
					return ammo;
				case Debugging.Particle:
					return particle;
				case Debugging.Camera:
					return level;
				case Debugging.Component:
					return component;
				case Debugging.Nodes:
					return nodes;
				default:
					return false;
			}
		}

		/// <summary>
		/// Toggle debugging states
		/// </summary>
		/// <param name="toggles">to toggle</param>
		private static void ToggleDebugState(Debugging[] toggles) {
			foreach(Debugging toggle in toggles) {
				switch (toggle) {
					case Debugging.Ammo:
						ammo = !ammo;
						break;
					case Debugging.Component:
						component = !component;
						break;
					case Debugging.Camera:
						level = !level;
						break;
					case Debugging.Particle:
						particle = !particle;
						break;
					case Debugging.Ship:
						ship = !ship;
						break;
					case Debugging.Nodes:
						nodes = !nodes;
						break;
				}
			}
		}

		/// <summary>
		/// Listen for player input and update debugging state
		/// </summary>
		public static void HandleInput() {
			if (SwinGame.KeyDown(KeyCode.CtrlKey)) {
				//debug all
				if (typed(KeyCode.DKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Ammo, Debugging.Component, Debugging.Camera, Debugging.Particle, Debugging.Ship, Debugging.Nodes });
				}
				//debug ship
				else if (typed(KeyCode.SKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Component, Debugging.Ship });
				}
				//debug ammo
				else if (typed(KeyCode.AKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Ammo });
				}
				//debug component
				else if (typed(KeyCode.CKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Component });
				}
				//debug level
				else if (typed(KeyCode.LKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Camera });
				}
				//debug particle
				else if (typed(KeyCode.PKey)) {
					ToggleDebugState(new Debugging[] { Debugging.Particle });
				}
			}
		}

		private static bool typed(KeyCode key) {
			return SwinGame.KeyTyped(key);
		}

	}
}
