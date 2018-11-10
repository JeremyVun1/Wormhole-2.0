using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForceUltra.src.GameModule
{
	/// <summary>
	/// Manages action/command steps and undoes them
	/// </summary>
	public class CommandHistory
	{
		private List<List<ICommand>> history;
		private int maxSize;
		private List<ICommand> currentStep;

		public CommandHistory(int maxSize) {
			this.maxSize = maxSize;
			history = new List<List<ICommand>>(maxSize);
			currentStep = new List<ICommand>();
		}

		public void NextStep() {
			//guards
			if (currentStep.Count == 0)
				return;
			if (history.Count >= maxSize)
				TrimCommandHistory();

			history.Add(currentStep);
			currentStep = new List<ICommand>();
		}

		public void AddCommand(ICommand c) {
			currentStep.Add(c);
		}

		public void UndoLastStep() {
			if (history.Count() == 0)
				return;

			List<ICommand> lastStep = history.Last();

			while (lastStep.Count > 0) {
				lastStep.Last().Undo();
				lastStep.RemoveAt(lastStep.Count - 1);
			}

			history.RemoveAt(history.Count - 1);
		}

		public bool HasSteps() {
			return (history.Count() > 0 ? true : false);
		}

		private void TrimCommandHistory() {
			while (history.Count >= maxSize) {
				history.Reverse();
				history.RemoveAt((history.Count - 1));
				history.Reverse();
			}
		}
	}
}
