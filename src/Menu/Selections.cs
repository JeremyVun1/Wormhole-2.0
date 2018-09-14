using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wormhole
{
	public class Selections
	{
		private string shipId;
		private string levelId;
		private string diffId;

		public void SetShip(string id)
		{
			shipId = id;
		}

		public void SetLevel(string id)
		{
			levelId = id;
		}

		public void SetDifficulty(string id)
		{
			diffId = id;
		}

		public bool AllSelected()
		{
			return (shipId != null && levelId != null && diffId != null);
		}

		public void Reset()
		{
			shipId = null;
			levelId = null;
			diffId = null;
		}

	}
}
