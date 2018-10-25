using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;
using TaskForceUltra.src.GameModule;

namespace TaskForceUltra
{
	/// <summary>
	/// Keeps track of game entities to update and draw them 
	/// Entity handler is used by many other objects
	/// </summary>
	public class EntityHandler : IHandlesEntities
	{
		public List<Entity> EntityList { get; private set; }
		private Scoresheet scoresheet;
		private Rectangle playArea;

		public EntityHandler(Scoresheet scoresheet, Rectangle playArea) {
			this.scoresheet = scoresheet;
			this.playArea = playArea;
			EntityList = new List<Entity>();
		}

		public void Update() {
			for (int i = EntityList.Count() - 1; i >= 0; --i) {
				EntityList[i].Update();

				if (EntityList[i].IsDead) {
					//score
					if (EntityList[i].KilledBy != Team.None) {
						scoresheet?.AddPoints(EntityList[i].KilledBy, EntityList[i].Mass);
					}

					//create debris
					List<LineSegment> lines = EntityList[i].DebrisLines;
					if (lines != null) {
						foreach (LineSegment l in lines) {
							Debris debris = new DebrisFactory().Create(l, EntityList[i].RealPos, playArea);
							Track(debris);
						}
					}

					Untrack(EntityList[i]);
				}
			}
		}

		public void Draw(Rectangle viewport) {
			//don't draw stuff unless it's within the view port
			foreach(Entity e in EntityList) {
				if (SwinGame.PointInRect(e.RealPos, viewport))
					e.Draw();
			}
		}

		public void Track(Entity entity) {
			if (entity != null)
				EntityList.Add(entity);
		}

		public void Untrack(Entity entity) {
			EntityList.Remove(entity);
		}

		/// <summary>
		/// returns how many AI ships are still alive
		/// </summary>
		public int AIShipCount() {
			int result = 0;

			foreach(AIShip aiShip in EntityList.OfType<AIShip>()) {
				result += 1;
			}
			return result;
		}
	}
}
