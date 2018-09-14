using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SwinGameSDK;

namespace Wormhole
{
	//ship is a movable entity
	public abstract class Ship : Entity, IShip
	{
		//general
		public int Cost { get; private set; }
		public float Condition
		{
			get { return (Health / BaseHealth); }
		}
		private Team team;

		//movement attributes
		private Vector vel;
		private Vector targetDir;

		//components
		private EngineGroup engines;
		private ToolGroup tools;
		private EmitterGroup emitters;

		//Handler
		private AmmoHandler ammoHandler;
		private ParticleHandler particleHandler;

		public Ship(JObject obj)
			: base(obj.Value<string>("id"),
				  obj.Value<int>("health"),
				  obj.GetValue("rgb"),
				  obj.GetValue("shape"),
				  obj.Value<float>("scale"))
		{
			Cost = obj.Value<int>("cost");

			particleHandler = new ParticleHandler(this);
			ammoHandler = new AmmoHandler(this);

			engines = new EngineGroup(obj.GetValue("engines"), particleHandler);
			tools = new ToolGroup(obj.GetValue("tools"), ammoHandler);
			emitters = new EmitterGroup(obj.GetValue("emitters"), particleHandler);

			vel = SwinGame.VectorTo(0, 0);
			Dir = SwinGame.VectorTo(0, -1);
			targetDir = Dir;
		}

		public override void Update()
		{
			base.Update();

			Move(vel);
			Rotate();
			ammoHandler.Update();
		}

		private void Rotate()
		{
		}

		private void Move()
		{
			Log.Pos(vel);
		}

		public override void Draw()
		{
			base.Draw();
			engines.Draw(Pos, Color);
			tools.Draw(Pos, Color);
			emitters.Draw(Pos, Color);
			ammoHandler.Draw();
			
		}
		
		//TODO
		//Split mass into it's own interface
		public int Mass
		{
			get { return Shape.Mass + engines.Mass + tools.Mass; }
		}

		public int RepairCost()
		{
			return Cost * (Health / BaseHealth);
		}

		//////////////////////
		//COLLISION
		//INTERFACE/API
		//////////////////////
		public override void ReactToCollision(int dmg, Point2D vel, Vector dir)
		{
			throw new NotImplementedException();
		}

		//////////////////////
		//MOVEMENT
		//INTEFACE/API
		//////////////////////
		//thrust along a vector dir
		//clamped to unit vector for throttle amount
		public void Thrust(Vector vDir)
		{
			vel = engines.ApplyThrust(vDir, vel, Mass);
		}

		//Rotation of concrete shapes is handled automatically in update()
		//turn target direction by an angle in degrees
		public void TurnBy(float theta)
		{
			targetDir = targetDir.Rotate(theta);
		}
		//set vector direction to turn to
		public void TurnTo(Vector dir)
		{
			targetDir = dir;
		}

		//shoot a specific weapon type
		public void Fire(string toolId)
		{
			tools.Activate<Weapon>(toolId);
		}
		//shoot all weapons
		public void Fire()
		{
			tools.Activate<Weapon>();
		}

		//teleport the ship to the specified point
		//method in the base class to maintain encapsulation of data
		public override void TeleportTo(Point2D target)
		{
			base.TeleportTo(target);
		}

		public void ActivatePowerup(PowerupType powerup)
		{
			//TODO
			throw new NotImplementedException();
		}

		public void SetTeam(Team team)
		{
			this.team = team;
		}

		public void SelfDestruct()
		{
			//TODO
			throw new NotImplementedException();
		}
	}
}