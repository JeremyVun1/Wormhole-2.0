using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra
{
	/// <summary>
	/// Interface for implementing Command pattern
	/// </summary>
	public interface ICommand
	{
		void Execute();
		void Undo();
	}
}
