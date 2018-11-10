using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using TaskForceUltra.src.GameModule;

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

		void TurnTo(Vector targetDir, float turnStrength = 1);
		bool ShouldThrust(Vector targetDir);
		void Thrust(Vector vDir);
		void Fire();
	}
}
