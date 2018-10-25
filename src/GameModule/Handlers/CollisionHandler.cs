using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskForceUltra.src.GameModule.Handlers;
using TaskForceUltra.src.GameModule.Entities;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Handles collisions between collideable entities
	/// </summary>
	public class CollisionHandler
	{
		private Node quadTree;
		private IHandlesEntities entityHandler;
		private NumberPopupFactory numberPopupFac;

		public CollisionHandler(Rectangle playArea, IHandlesEntities entityHandler) {
			this.entityHandler = entityHandler;
			quadTree = new Node(null, playArea, 150);
			numberPopupFac = new NumberPopupFactory(playArea);
		}

		public void Update() {
			RegisterAll();
			CollideEntities();

			quadTree.DebugDraw();
		}

		/// <summary>
		/// Register all entities being tracked by the passed in entity handler into the collision tree
		/// </summary>
		public void RegisterAll() {
			quadTree.Clear();
			foreach (ICollides c in entityHandler.EntityList.OfType<ICollides>()) {
				quadTree.Register(c);
			}
		}

		/// <summary>
		/// Run collision checking on all entities
		/// </summary>
		private void CollideEntities() {
			quadTree.CheckedList.Clear();
			List<NumberPopup> popups = new List<NumberPopup>();
			
			foreach (ICollides self in entityHandler.EntityList.OfType<ICollides>()) {
				ICollides other = quadTree.CollidingWith(self);

				if (other != null) {
					bool selfCollided = self.TryReactToCollision(other.Damage, other.Vel, other.Mass, other.Team, other is Ammo);
					bool otherCollided = other.TryReactToCollision(self.Damage, self.Vel, self.Mass, self.Team, self is Ammo);

					if (selfCollided)
						popups.Add(numberPopupFac.Create(self.RealPos, other.Damage));
					if (otherCollided)
						popups.Add(numberPopupFac.Create(other.RealPos, self.Damage));
				}
			}

			foreach (NumberPopup p in popups) {
				entityHandler.Track(p);
			}
		}
	}
}
