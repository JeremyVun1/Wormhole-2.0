using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SwinGameSDK;

namespace Wormhole
{
	public class PlayerShip : Ship, IControllableShip
	{
		public PlayerShip(dynamic obj) : base((JObject)obj)
		{

		}


		public void Forward()
		{
			//get vector direction and pass it into here
			Vector thrustDir = SwinGame.VectorTo(0, 0);
			thrustDir += SwinGame.VectorTo(0, -1);

			double theta = Dir.Angle + 90;
			thrustDir = thrustDir.Rotate(theta);

			Thrust(thrustDir);
		}

		public void Backward()
		{
			//get vector direction and pass it into here
			Vector thrustDir = SwinGame.VectorTo(0, 0);
			thrustDir += SwinGame.VectorTo(0, 1);

			double theta = Dir.Angle + 90;
			thrustDir = thrustDir.Rotate(theta);

			Thrust(thrustDir);
		}

		public void StrafeLeft()
		{
			Vector v = SwinGame.VectorTo(-1, 0);
			v.Rotate(Math.PI / 2);

			v = v.Rotate(Dir.Angle + 90);

			Thrust(v);
		}

		public void StrafeRight()
		{
			Vector v = SwinGame.VectorTo(1, 0);
			v.Rotate(Math.PI / 2);

			v = v.Rotate(Dir.Angle + 90);

			Thrust(v);
		}

		public void TurnLeft()
		{
			
		}

		public void TurnRight()
		{
			
		}

		public void ActivatePowerup()
		{

		}

		public void Shoot()
		{
			Fire();
		}
	}
}
