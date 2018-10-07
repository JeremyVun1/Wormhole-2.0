using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra
{
	public interface IModuleInterface : IReceiveMenuData, IReceiveGameData
	{
	}

	public interface IReceiveGameData
	{
		void ReceiveGameData(Dictionary<GameResultType, int> receiveData);
	}

	public interface IReceiveMenuData
	{
		void ReceiveMenuData(Dictionary<SelectionType, string> receiveData);
	}
}
