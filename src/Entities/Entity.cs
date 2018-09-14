using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Stateless;

namespace Wormhole
{
	//entity is something that has a shape, color and can be drawn to the screen
	//not something that can move
	public abstract class Entity : ICollidable, ITeleports
	{
		public string Id { get; private set; }
		protected float Scale { get; private set; }
		protected Shape Shape { get; private set; }
		public Vector Dir { get; protected set; }
		protected Color Color { get; private set; }
		public Rectangle BoundingBox
		{
			get { return Shape.BoundingBox; }
		}
		
		public Point2D Pos { get; private set; }

		protected int BaseHealth { get; set; }
		protected int Health { get; set; }
		private Timer collisionTimer;
		private int hurtCooldown;

		private enum State { REST, HURT };
		private enum Trigger { COLLIDE, RESET };
		private StateMachine<State, Trigger> stateMachine;


		public Entity(string id, int health, dynamic rgb, dynamic shape, dynamic scale)
		{
			//populate attributes
			Id = id;
			Color = SwinGame.RGBColor((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]);
			Scale = scale;
			Shape = new Shape(shape, scale);
			BaseHealth = health;
			Health = health;
			hurtCooldown = 2000; //hardcode 2 seconds for now

			collisionTimer = SwinGame.CreateTimer();
			stateMachine = new StateMachine<State, Trigger>(State.REST);
			ConfigureStateMachine();
		}

		private void ConfigureStateMachine()
		{
			stateMachine.Configure(State.REST)
				.OnEntry(() => collisionTimer.Stop())
				.Permit(Trigger.COLLIDE, State.HURT)
				.Ignore(Trigger.RESET);
			stateMachine.Configure(State.HURT)
				.OnEntry(() => collisionTimer.Start())
				.Permit(Trigger.RESET, State.REST)
				.Ignore(Trigger.COLLIDE);
		}

		public virtual void Update()
		{
			switch (stateMachine.State)
			{
				case State.REST:
					break;
				case State.HURT:
					if (collisionTimer.Ticks > hurtCooldown)
						stateMachine.Fire(Trigger.RESET);
					break;
			}
		}

		protected void Move(Vector vel)
		{
			Pos = Pos.Add(vel);
		}

		public virtual void Draw()
		{
			Shape.Draw(Pos, Color);
			//SwinGame.FillRectangle(Color.Aqua, Shape.BoundingBox);
		}

		public bool CollidingWith(ICollidable other)
		{
			if (CanCollide() && SwinGame.RectanglesIntersect(this.BoundingBox, other.BoundingBox))
			{
				stateMachine.Fire(Trigger.COLLIDE);
				return true;
			}
			return false;
		}

		//put conditions for collision here e.g. invuln or hurt cooldown
		protected virtual bool CanCollide()
		{
			return stateMachine.State != State.HURT;
		}

		public abstract void ReactToCollision(int dmg, Point2D vel, Vector dir);

		public virtual void TeleportTo(Point2D target)
		{
			Shape.TeleportTo(target);
			Pos = target;
		}
	}
}
