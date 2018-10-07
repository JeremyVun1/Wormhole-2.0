using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule
{
	public abstract class Entity : PositionedObject
	{
		public string Id { get; private set; }
		public Team Team { get; protected set; }
		public string FilePath { get; private set; }

		protected int baseHealth;
		protected int health;
		public float Condition {
			get { return (health / baseHealth); }
		}
		public bool IsDead { get; protected set; }
		public Team KilledBy { get; protected set; }

		public Shape Shape { get; private set; }
		public virtual List<LineSegment> DebrisLines { get { return Shape.GetLines(); } }
		protected List<Color> colors;
		protected int colorIndex;

		protected bool debugMode;
		
		public List<LineSegment> BoundingBox { get { return Shape?.BoundingBox; } }
		public virtual int Mass {
			get	{
				if (Shape != null)
					return Shape.Mass;
				else return 1;
			}
		}

		public Entity(
			string id, string filePath, Point2D refPos, Point2D offsetPos,
			Shape shape, List<Color> colors, int health, Team team
		) : base(refPos, offsetPos)
		{
			Id = id;
			Team = team;
			FilePath = filePath;
			baseHealth = this.health = health;
			Shape = shape;
			this.colors = colors;
			colorIndex = 0;

			debugMode = false;
			IsDead = false;
		}

		public virtual void Update() {
			DebugMode();
		}

		public virtual void Draw() {
			Shape?.Draw(colors[colorIndex]);

			if (debugMode) {
				Debug();
			}
		}

		private void DebugMode() {
			if (SwinGame.KeyTyped(KeyCode.TKey)) {
				debugMode = !debugMode;
				Console.WriteLine("DEBUG TOGGLED");
			}
		}

		public override void TeleportTo(Point2D target) {
			base.TeleportTo(target);
			Shape?.TeleportTo(target);
		}

		public virtual void Kill(Team killer) {
			IsDead = true;
			KilledBy = killer;
		}

		protected virtual void Debug() {
			Shape?.Debug(Color.Red);
			SwinGame.FillRectangle(Color.Blue, refPos.X, refPos.Y, 5, 5);


			//Log.Pos($"{Id}: refPos", refPos);
			//Log.Pos("offsetPos", offsetPos);
			//Log.Pos("RealPos", RealPos);
		}
	}
}