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

		public int Count { get { return history.Count; } }

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
			if (history.Count() == 0) {
				history.Clear();
				return;
			}

			//grab the last step
			List<ICommand> lastStep = history.Last();

			while (lastStep.Count > 0) {
				lastStep.Last().Undo();
				lastStep.RemoveAt(lastStep.Count - 1);
			}

			lastStep.Clear();
			history.RemoveAt(history.Count - 1);
		}

		public bool HasSteps() {
			return (history.Count() > 0 ? true : false);
		}

		private void TrimCommandHistory() {
			for (int i=0; i < history.Count-1; i++) {
				history[i] = history[i+1];
			}
			history.RemoveAt((history.Count - 1));
		}
	}
}
