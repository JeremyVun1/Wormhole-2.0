using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using SwinGameSDK;

namespace Wormhole
{
	public class Engine : Component
	{
		private float thrust;
		private float maxVel;
		private float turnRate;
		private ParticleHandler particleHandler;

		private bool thrusting;

		public Engine(ParticleHandler handler)
		{
			particleHandler = handler;
		}

		public override void Init(JObject obj)
		{
			thrust = obj.Value<float>("thrust");
			maxVel = obj.Value<float>("maxVel");
			turnRate = obj.Value<float>("turnRate");

			ChildComponents = new EmitterGroup(obj.GetValue("emitters"), particleHandler);

			base.Init(obj);
		}

		//return clamped vector modified by vDir
		public Vector ApplyForce(Vector vDir, Vector vel, int Mass)
		{
			//apply force to the vector
			Vector modified = vel;
			Vector force = new Vector
			{
				X = vDir.X * (thrust / Mass),
				Y = vDir.Y * (thrust / Mass)
			};
			modified += force;

			if (modified.Magnitude > maxVel)
				return vel;
			return modified;
		}

		public override void Update()
		{
			if (thrusting)
			{
				ChildComponents.Activate<Emitter>();
				thrusting = false;
			}

			base.Update();
		}
	}
}
