using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// bundles game result data to send to the game controller
	/// </summary>
	public class GameSendData
	{
		private IReceiveGameData receiver;
		private Dictionary<GameResultType, int> results;

		public GameSendData(IReceiveGameData receiver) {
			results = new Dictionary<GameResultType, int>();
			this.receiver = receiver;
		}

		public void Send() {
			if (results.Count() == Enum.GetNames(typeof(SelectionType)).Length)
				receiver.ReceiveGameData(results);
		}

		public void Add(GameResultType result, int x) {
			if (results.ContainsKey(result))
				Remove(result);
			results.Add(result, x);
		}

		public void Remove(GameResultType result) {
			results.Remove(result);
		}
	}
}
