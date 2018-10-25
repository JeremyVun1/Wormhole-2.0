using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using Newtonsoft.Json;
using System.IO;

namespace TaskForceUltra
{
	/// <summary>
	/// Player moneys and id
	/// </summary>
	public class Bank
	{
		[JsonProperty("id")]
		public string PlayerName { get; private set; }
		[JsonProperty("credits")]
		public int Credits { get; private set; }
		[JsonIgnore]
		private string filePath;

		public Bank(string filePath, int credits = 0) {
			Credits = credits;
			this.filePath = filePath;
			Load();
		}

		public void AddCredits(int credits) {
			Credits += credits;
		}

		public void RemoveCredits(int credits) {
			AddCredits(-credits);
			Credits = credits.Clamp(0, Credits);
		}

		/// <summary>
		/// Load from file
		/// </summary>
		public void Load() {
			try {
				string jsonStr = File.ReadAllText(filePath);

				JsonConvert.PopulateObject(jsonStr, this);
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}

		/// <summary>
		/// Save to file
		/// </summary>
		public void Save() {
			try {
				string jsonStr = JsonConvert.SerializeObject(this);
				Console.WriteLine(jsonStr);
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}
	}
}
