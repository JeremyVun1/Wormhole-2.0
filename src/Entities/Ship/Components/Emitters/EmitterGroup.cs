using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wormhole
{
	public class EmitterGroup : ComponentGroup
	{
		protected string emitterDir;

		public EmitterGroup(dynamic emittersJArr) : base()
		{
			emitterDir = resourcePath + "\\emitters";

			List<Emitter> temp = emittersJArr.ToObject<List<Emitter>>();
			components = temp.ToList<IComponent>();

			InitComponents();
		}
	}
}
