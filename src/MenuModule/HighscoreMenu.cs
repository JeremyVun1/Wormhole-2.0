using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using System.IO;

namespace TaskForceUltra.src.MenuModule
{
	public class HighscoreMenu : Menu
	{
		private Dictionary<string, int> highscores;

		public HighscoreMenu(string id, string title, List<MenuElement> elements) : base(id, title, elements) {
			highscores = new Dictionary<string, int>();
			PopulateHighScores();

			Save();
		}

		public override void Update() {
			base.Update();

			for (int i=0; i<highscores.Count; ++i) {
				InsertText($"name{i}", highscores.ElementAt(i).Key);
				InsertText($"score{i}", highscores.ElementAt(i).Value.ToString());
			}
		}

		public void PopulateHighScores() {
			int i = 0;

			foreach (TextBox t in elements.OfType<TextBox>()) {
				string key = $"empty{i}";
				int value = 0;

				if (t.id == $"name{i}") {
					key = t.text;
				}
				else if (t.id == $"score{i}") {
					Int32.TryParse(t.text, out value);
				}

				highscores.Add(key, value);
				i += 1;
			}
		}

		public bool IsHighscore(int score) {
			for(int i=0; i<highscores.Count; ++i) {
				if (score > highscores.ElementAt(i).Value)
					return true;
			}
			return false;
		}

		public void InsertScore(string name, int score) {
			for(int i=0; i<highscores.Count; ++i) {
				if (score >= highscores.ElementAt(i).Value) {
					ShiftScoresDown(i);
					highscores[highscores.ElementAt(i).Key] = score;
				}
			}

			Save();
		}

		public void Save() {
			List<TextBox> highscores = new List<TextBox>();
			foreach (TextBox t in elements.OfType<TextBox>()) {
				highscores.Add(t);
			}

			string jsonString = JsonConvert.SerializeObject(highscores, Formatting.Indented);
			File.WriteAllText(SwinGame.AppPath() + "\\resources\\data\\scores.json", jsonString);
		}

		private void ShiftScoresDown(int index) {
			for(int i = index + 1; i<highscores.Count; ++i) {
				highscores[highscores.ElementAt(i).Key] = highscores[highscores.ElementAt(i - 1).Key];
			}
		}
	}
}
