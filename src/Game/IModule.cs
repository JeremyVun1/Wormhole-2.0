using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public interface IModule
	{
		bool Ended { get; }
		Player PlayerProgress { get; }
		//void UpdateProgress();
		void Update();
		void Draw();
	}
}
