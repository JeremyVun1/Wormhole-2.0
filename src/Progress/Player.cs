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
		private int _money;
		private Dictionary<IShip, int> _ownedShips; //list of ships that the player owns and their condition in %
		private int _levelProgress;
		private DifficultyType _difficulty;
		private string _filePath;

		public void AddMoney(int x) => _money += x;
		public void RemoveMoney(int x) => _money -= x;
		public int Balance() { return _money; }

		public void AddShip(IShip ship)
		{
			if (!Owned(ship))
				_ownedShips.Add(ship, 100);
		}

		public void SetShipCondition(IShip ship, int cond)
		{
			if (Owned(ship))
				_ownedShips[ship] = cond.Clamp(0, 100);
		}

		public void IncLevelProgress(int x) => _levelProgress = x;

		public void SetDifficulty(DifficultyType d) => _difficulty = d;

		//load progress from file
		private void LoadProgress()
		{
			if (File.Exists(_filePath))
			{
				try
				{
					string buffer = File.ReadAllText(_filePath);
					JsonConvert.PopulateObject(buffer, this);
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
			try
			{
				string buffer = JsonConvert.SerializeObject(this);
				File.WriteAllText(_filePath, buffer);
			} catch (Exception e)
			{
				Log.Ex(e, "error saving player progress to file");
			}
		}

		public Player()
		{
			//set filepath
			_filePath = SwinGame.AppPath() + "\\Resources\\player.json";

			LoadProgress();
		}

		private bool Owned(IShip ship)
		{
			return _ownedShips.ContainsKey(ship);
		}
	}
}