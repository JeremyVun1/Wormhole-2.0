using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;

namespace TaskForceUltra.src.GameModule.AI
{
	public interface IAIEntity
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
