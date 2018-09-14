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
	public class Emitter : Component
	{
		private float emissionRate;
		private string particleId;
		private ParticleHandler particleHandler;

		public Emitter(ParticleHandler handler)
		{
			particleHandler = handler;
		}

		public override void Init(JObject obj)
		{
			emissionRate = obj.Value<float>("rate");
			cdHandler = new CooldownHandler(1/emissionRate);

			ChildComponents = new ParticleGroup(obj.GetValue("particle"));
			particleId = obj.GetValue("particle").Value<string>("id");

			base.Init(obj);
		}

		public override void Update()
		{
			cdHandler.Update();
		}

		public override void Draw(Point2D parentPos, Color parentColor)
		{
			//TODO colors array is null?
			base.Draw(parentPos, parentColor);
		}

		public override void Activate()
		{
			if (cdHandler.OnCooldown())
				return;

			//fetch template particle
			Particle templateParticle = (Particle)ChildComponents.FetchComponent<Particle>(particleId);
			//shallow copy
			Particle newParticle = templateParticle.Clone();

			particleHandler.AddParticle(newParticle);
			cdHandler.StartCooldown();
		}
	}
}
