using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra.src.GameModule.Entities
{
	public class Emitter : Component
	{
		public Particle Particle { get { return childComponents.OfType<Particle>().First(); } }
		private IHandlesEntities entHandler;

		private float cooldownRate;
		private CooldownHandler cdHandler;

		public Emitter(
			string id, string filePath, Point2D refPos, Point2D offsetPos,
			Vector vel, Vector dir, BoundaryStrategy boundaryStrat, Team team,
			float cooldownRate, List<Component> children, IHandlesEntities entHandler
		) : base(id, filePath, refPos, offsetPos, null, null, 1, vel, dir, boundaryStrat, team)
		{
			childComponents = children;
			this.cooldownRate = cooldownRate;
			cdHandler = new CooldownHandler(1000 / cooldownRate);
			this.entHandler = entHandler;
		}

		public void Activate() {
			//create new particle, init it, track it
			if (!cdHandler.OnCooldown()) {
				JObject particleObj = Util.Deserialize(Particle.FilePath);

				Particle newParticle = new ParticleFactory().Create(particleObj, FilePath, entHandler, boundaryStrat, Team, offsetPos) as Particle;

				newParticle.Init(refPos, Dir);
				entHandler.Track(newParticle);
				cdHandler.StartCooldown();
			}
		}

		public override void Update() {
			cdHandler.Update();
			base.Update();
		}
	}

	/// <summary>
	/// Emitter Factory
	/// </summary>
	public class EmitterFactory : ComponentFactory
	{
		public override Component CreateFromReference(JObject emitterObj, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D parentPos, float mod =1) {
			Point2D offsetPos = emitterObj["pos"].ToObject<Point2D>().Multiply(10);
			offsetPos = offsetPos.Add(parentPos);

			return base.CreateFromReference(emitterObj, entHandler, boundaryStrat, team, offsetPos);
		}

		public override Component Create(JObject emitterObj, string path, IHandlesEntities entHandler, BoundaryStrategy boundaryStrat, Team team, Point2D offsetPos, float mod =1) {
			string id = emitterObj.Value<string>("id");
			float cooldownRate = emitterObj.Value<float>("rate");

			JObject particleObj = emitterObj.Value<JObject>("particle");
			Component particle = new ParticleFactory().CreateFromReference(particleObj, entHandler, boundaryStrat, team, offsetPos);

			return new Emitter(id, path, SwinGame.PointAt(0, 0), offsetPos, SwinGame.VectorTo(0, 0), SwinGame.VectorTo(0, -1),
				boundaryStrat, team, cooldownRate, new List<Component>() { particle }, entHandler);
		}
	}
}
