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
	public class Particle : Component, ITeleports
	{
		private float friction;
		private int colorIndex;
		private MinMax<float> velRange;
		private MinMax<float> lifetimeRange;
		private MinMax<float> turnRateRange;
		private float vel;
		private float lifetime;
		private Random rng;

		//public Emitter Emitter { get; set; }

		public override void Init(JObject obj)
		{
			rng = new Random(Guid.NewGuid().GetHashCode());

			JToken temp = obj.GetValue("lifetimeRange");
			lifetimeRange = new MinMax<float>(temp.Value<float>("min"), temp.Value<float>("max"));

			temp = obj.GetValue("turnRateRange");
			turnRateRange = new MinMax<float>(temp.Value<float>("min"), temp.Value<float>("max"));

			temp = obj.GetValue("velRange");
			velRange = new MinMax<float>(temp.Value<float>("min"), temp.Value<float>("max"));

			friction = obj.Value<float>("friction");

			base.Init(obj);

			RandomiseAttributes();
			cdHandler = new CooldownHandler(lifetime);			
		}

		public Particle Clone()
		{
			Particle result = (Particle)MemberwiseClone();
			result.RandomiseAttributes();

			return result;
		}

		private void RandomiseAttributes()
		{
			vel = Util.RandomRange(velRange);
			lifetime = Util.RandomRange(lifetimeRange);
			turnRate = Util.RandomRange(turnRateRange);
			colorIndex = SwinGame.Rnd(colors.Count);
		}

		public override void Update()
		{
			if (cdHandler.OnCooldown())
			{
				Move();
			}
			else Dead = true;
		}

		public override void Draw(Point2D p, Color c)
		{
			if (cdHandler.OnCooldown())
			{
				base.Draw(Pos, colors[colorIndex]);
			}
		}

		private void Move()
		{
			Pos += Dir.UnitVector.Multiply(vel);
			vel *= friction;
		}

		public void TeleportTo(Point2D target)
		{
			Pos = target;
			shape.TeleportTo(target);
		}

		//set vector direction to turn to
		public void TurnTo(Vector dir)
		{
			TargetDir = dir;
		}
	}
}
