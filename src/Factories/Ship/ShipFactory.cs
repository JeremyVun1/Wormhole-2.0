using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Wormhole
{
	public class ShipFactory
	{
		string[] fileList;
		private string resourceDir { get; set; }
		private ShipList shipList { get; set; }

		public ShipFactory(string dir)
		{
			resourceDir = dir;
			shipList = new ShipList();
		}

		public ShipList BuildShipList()
		{
			shipList.Clear();

			//get list of all files
			string buffer;
			fileList = Directory.GetFiles(resourceDir);

			foreach(string file in fileList)
			{
				buffer = File.ReadAllText(file);
				dynamic obj = JsonConvert.DeserializeObject(buffer);

				//build ship
				Ship s = new Ship(obj);

				//add ship to list
				shipList.Add(s);
			}

			return shipList;
		}

		public IShip Fetch(string shipId)
		{
			return shipList.Fetch(shipId);
		}
	}
}
