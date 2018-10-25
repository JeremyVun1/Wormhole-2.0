using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// interface for tracking and untracking entities from an entity handler
	/// </summary>
	public interface IHandlesEntities
	{
		List<Entity> EntityList { get; }

		void Track(Entity entity);
		void Untrack(Entity entity);
	}
}
