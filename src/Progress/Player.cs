using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SwinGameSDK;
using Newtonsoft.Json;

namespace Wormhole
{
	public class Player
	{
		[JsonProperty]
		public string Id;
		[JsonProperty]
		private int money;
		[JsonProperty]
		private Dictionary<string, float> ownedShips; //list of ships that the player owns and their condition in %
		[JsonProperty]
		private int levelProgress;
		[JsonProperty]
		private DifficultyType difficulty;
		private string progressPath;

		public void AddMoney(int x) => money += x;
		public void RemoveMoney(int x) => money -= x;


		public int Balance() { return money; }

		public void TestAddShip(string id, float condition)
		{
			ownedShips.Add(id, condition);
		}

		public void AddShip(IShip ship)
		{
			if (!Owned(ship))
				ownedShips.Add(ship.Id, ship.Condition);
		}

		public void SetShipCondition(IShip ship, float cond)
		{
			if (Owned(ship))
				ownedShips[ship.Id] = cond.Clamp(0, 1);
		}

		public void IncLevelProgress(int x) => levelProgress = x;

		public void SetDifficulty(DifficultyType d) => difficulty = d;

		//load progress from file
		private void LoadProgress()
		{
			Console.WriteLine(File.Exists(progressPath));
			if (File.Exists(progressPath))
			{
				try
				{
					string buffer = File.ReadAllText(progressPath);
					JsonConvert.PopulateObject(buffer, this);
					PopulateFields(JsonConvert.DeserializeObject(buffer));
				}
				catch (Exception e)
				{
					Log.Ex(e, "error loading player progress from file");
				}
				return;
			}
			else SaveProgress(); //create file if it doens't exist;
		}

		//save progress to file
		public void SaveProgress()
		{
			if (File.Exists(progressPath))
			{
				try
				{
					string buffer = JsonConvert.SerializeObject(this);
					JsonSerializerSettings a = new JsonSerializerSettings();
					string path = SwinGame.AppPath() + "\\testSave.json";
					File.WriteAllText(path, buffer);
				}
				catch (Exception e)
				{
					Log.Ex(e, "error saving player progress to file");
				}
			}			
		}

		public Player(dynamic obj)
		{
			PopulateFields(obj);
		}
		public Player(string path)
		{
			progressPath = path;
			LoadProgress();
		}

		private void PopulateFields(dynamic obj)
		{
			Id = obj.id;
			money = obj.money;

			if (ownedShips == null)
			{
				ownedShips = new Dictionary<string, float>();
			}

			Log.Msg("hi");
			foreach (var s in obj.ownedShips)
			{
				Console.WriteLine(s.id);
				Console.WriteLine(s.condition);
				ownedShips.Add((string)s.id, (float)s.condition);
			}
			levelProgress = obj.levelProgress;
			difficulty = (DifficultyType) Enum.Parse(typeof(DifficultyType), (string)obj.difficulty);
		}

		private bool Owned(IShip ship)
		{
			return ownedShips.ContainsKey(ship.Id);
		}
	}
}