using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.AI
{
	/// <summary>
	/// AI entity interface for being controlled by an AI strategy
	/// </summary>
	public interface IAIEntity : IControllable
	{
		bool IsDead { get; }
		Point2D RealPos { get; }
		Team Team { get; }
		Vector Vel { get; }
		float MaxVel { get; }
		Vector Dir { get; }

		bool ShouldThrust(Vector targetDir);
		void Thrust(Vector vDir);
		void Fire();
	}
}
