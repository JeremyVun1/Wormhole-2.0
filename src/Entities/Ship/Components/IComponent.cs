using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SwinGameSDK;

namespace Wormhole
{
	public interface IComponent
	{
		void Activate();
		void Update();
		void Draw(Point2D pos, Color color);
		void Init(JObject obj);
		string Path { get; }
		int Mass { get; }
		bool AreYou(string id);
	}
}
