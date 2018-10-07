using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule.Entities;

namespace TaskForceUltra.src.GameModule
{
	public class EntityHandler : IHandlesEntities
	{
		public List<Entity> EntityList { get; private set; }
		private Scoresheet scoresheet;

		public EntityHandler(Scoresheet scoresheet) {
			this.scoresheet = scoresheet;
			EntityList = new List<Entity>();
		}

		public void Update() {
			for (int i = EntityList.Count() - 1; i >= 0; --i) {
				EntityList[i].Update();

				if (EntityList[i].IsDead) {
					//score
					if (EntityList[i].KilledBy != Team.None) {
						scoresheet.AddPoints(EntityList[i].KilledBy, EntityList[i].Mass);
					}

					//create debris
					List<LineSegment> lines = EntityList[i].DebrisLines;
					if (lines != null) {
						foreach (LineSegment l in lines) {
							Debris debris = new DebrisFactory().Create(l, EntityList[i].RealPos);
							Track(debris);
						}
					}

					Untrack(EntityList[i]);
				}
			}
		}

		public void Draw() {
			foreach(Entity e in EntityList) {
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
	}
}
