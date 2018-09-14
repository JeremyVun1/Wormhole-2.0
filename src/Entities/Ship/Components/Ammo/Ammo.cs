using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json.Linq;

namespace Wormhole
{
	public class Ammo : Component, ITeleports
	{
		private int damage;
		private float lifetime;
		private float vel;
		private IAIStrategy aiStrategy;

		public override void Init(JObject obj)
		{
			colors = new List<Color>();

			damage = 1;
			lifetime = obj.Value<float>("lifetime");
			turnRate = obj.Value<float>("turnRate");
			cdHandler = new CooldownHandler(lifetime);
			vel = obj.Value<float>("vel");

			base.Init(obj);
		}

		public Ammo Clone()
		{
			return (Ammo)MemberwiseClone();
		}

		public override void Update()
		{
			if (cdHandler.OnCooldown())
			{
				Move();
				aiStrategy?.Update();
			}
			else Dead = true;
		}

		private void Move()
		{
			Pos += Dir.UnitVector.Multiply(vel);
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
