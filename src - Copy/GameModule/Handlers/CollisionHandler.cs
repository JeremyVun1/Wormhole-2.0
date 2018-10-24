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

		public CollisionHandler(Rectangle playArea, IHandlesEntities entityHandler) {
			this.entityHandler = entityHandler;
			quadTree = new Node(null, playArea, 100);
		}

		public void Update() {
			RegisterAll();
			CollideEntities();
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
			
			foreach (ICollides self in entityHandler.EntityList.OfType<ICollides>()) {
				ICollides other = quadTree.CollidingWith(self);

				//returns
				if (other != null) {
					self.ReactToCollision(other.Damage, other.Vel, other.Mass, other.Team, other is Ammo);
					other.ReactToCollision(self.Damage, self.Vel, self.Mass, self.Team, self is Ammo);
				}
			}
		}
	}
}
