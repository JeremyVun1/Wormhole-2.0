using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TaskForceUltra.src.MenuModule
{
	public class HighscoreMenu : Menu
	{
		private string scoresFile;
		private List<KeyValuePair<string, int>> highscores;

		public HighscoreMenu(string id, string title, List<MenuElement> elements) : base(id, title, elements) {
			highscores = new List<KeyValuePair<string, int>>();
			scoresFile = SwinGame.AppPath() + "\\resources\\data\\scores.json";

			PopulateHighscores();

			Load();
		}

		public override void Update() {
			base.Update();
		}

		private void PopulateHighscores() {
			string key = "";
			int count = 0, i = 0, value = 0;

			//iterate through textboxes and get values
			foreach (TextBox t in elements.OfType<TextBox>()) {
				if (t.id == $"name{i}") {
					key = t.text;
					count += 1;
				}
				else if (t.id == $"score{i}") {
					Int32.TryParse(t.text, out value);
					count += 1;
				}

				if (count == 2) {
					KeyValuePair<string, int> entry = new KeyValuePair<string, int>(key, value);
					highscores.Add(entry);
					count = 0;
					i += 1;
				}
			}
		}

		private void UpdateTextBoxes() {
			for (int i = 0; i < highscores.Count; ++i) {
				InsertText($"name{i}", highscores.ElementAt(i).Key);
				InsertText($"score{i}", highscores.ElementAt(i).Value.ToString());
			}
		}

		private bool IsHighscore(int points) {
			if (highscores.Count < 10)
				return true;

			for(int i=0; i<highscores.Count; ++i) {
				if (points > highscores.ElementAt(i).Value)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Attempts to insert score into the high scores
		/// </summary>
		/// <param name="name">name of the player</param>
		/// <param name="points">score they got</param>
		/// <returns>returns true if got a highscore, false if not highscore</returns>
		public bool TryInsertScore(string name, int points) {
			if (!IsHighscore(points))
				return false;

			highscores.Add(new KeyValuePair<string, int>(name, points));

			SortTrimHighscores();
			UpdateTextBoxes();

			Save();
			return true;
		}

		/// <summary>
		/// Order highscores descending and remove lowest score
		/// </summary>
		private void SortTrimHighscores() {
			var sorted = highscores.OrderByDescending(x => x.Value);
			int numScores = sorted.Count() < 10 ? sorted.Count() : 10;

			List<KeyValuePair<string, int>> newHighscores = new List<KeyValuePair<string, int>>();
			for (int i = 0; i < numScores; i++) {
				var result = new KeyValuePair<string, int>(sorted.ElementAt(i).Key, sorted.ElementAt(i).Value);
				newHighscores.Add(result);
			}
			highscores = newHighscores;
		}

		/// <summary>
		/// save dictionary to file
		/// </summary>
		private void Save() {
			var textBoxes = elements.OfType<TextBox>();

			string jsonString = JsonConvert.SerializeObject(textBoxes, Formatting.Indented);
			File.WriteAllText(scoresFile, jsonString);
		}

		/// <summary>
		/// load dictionary from file
		/// </summary>
		private void Load() {
			string jsonString = File.ReadAllText(scoresFile);
			JArray obj = JArray.Parse(jsonString);

			/*highscores.Clear();
			foreach (JObject entry in obj.Children<JObject>()) {
				foreach (JProperty p in entry.Properties()) {
					highscores.Add(p.Name, (int)p.Value);
				}
			}*/
		}
	}
}
