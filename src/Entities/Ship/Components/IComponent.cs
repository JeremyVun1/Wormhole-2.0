using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public interface IComponent
	{
		void Update();
		void Draw();
		void Init(dynamic obj);
		string Path { get; }
		int Mass { get; }
		bool AreYou(string id);
	}
}
