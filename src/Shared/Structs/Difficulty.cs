using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwinGameSDK;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaskForceUltra.src.GameModule.AI;

namespace TaskForceUltra
{
	public struct Difficulty
	{
		public string Id;
		public float SpawnTimer;
		public float AIMod;
		public int DifficultyLevel;
		public int ShootCooldown;

		public Difficulty(string id, float spawnTimer, float aiMod, int diffLevel, int shootCooldown) {
			Id = id;
			SpawnTimer = spawnTimer;
			AIMod = aiMod;
			DifficultyLevel = diffLevel;
			ShootCooldown = shootCooldown;
		}
	}

	/// <summary>
	/// static accessor for Difficulty settings. Might as well try having one for fun
	/// </summary>
	//prevent inheritance
	public static class DifficultySetting
	{
		private static string dirPath = SwinGame.AppPath() + "\\resources\\data\\difficulty";

		public static Difficulty Fetch(string id) {
			switch (id.ToLower()) {
				case "easy":
					return Easy;
				case "medium":
					return Medium;
				case "hard":
					return Hard;
				default: return Easy;
			}
		}

		//difficulty properties
		public static Difficulty Easy {
			get { return ReadDifficulty(dirPath + "\\easy.json"); } }
		public static Difficulty Medium {
			get { return ReadDifficulty(dirPath + "\\medium.json"); } }
		public static Difficulty Hard {
			get { return ReadDifficulty(dirPath + "\\hard.json"); } }

		private static Difficulty ReadDifficulty(string filePath) {
			JObject diffObj = Util.Deserialize(filePath);

			string id = diffObj.Value<string>("difficultyId");
			float spawnTimer = diffObj.Value<float>("spawnTimer");
			float aiMod = diffObj.Value<float>("aiMod");
			int diffLevel = diffObj.Value<int>("difficultyLevel");
			int shootCooldown = diffObj.Value<int>("shootCooldown");

			return new Difficulty(id, spawnTimer, aiMod, diffLevel, shootCooldown);
		}
	}
}
