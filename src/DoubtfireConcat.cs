using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SwinGameSDK;

namespace TaskForceUltra.src
{
	public class DoubtfireConcat
	{
		private string srcPath = "C:\\Users\\elode\\Documents\\swinburne\\2018_sem2\\COS20007 oop\\custom proj\\TaskForceUltra\\src";

		List<string> filenames = new List<string>();

		public void Run() {
			BuildFilenames();

			Write();
		}

		private void Write() {
			string[] output = new string[filenames.Count];

			for(int i=0; i<output.Count(); ++i) {
				output[i] = File.ReadAllText(filenames[i]);
			}

			File.WriteAllLines("C:\\Users\\elode\\Documents\\swinburne\\2018_sem2\\COS20007 oop\\custom proj\\TaskForceUltra\\output.cs", output);
		}

		private void BuildFilenames() {
			AddFilesFromDir(srcPath);
			AddFilesFromDir(srcPath + "\\Enums");

			AddFilesFromDir(srcPath + "\\GameModule");
			AddFilesFromDir(srcPath + "\\GameModule\\AI");
			AddFilesFromDir(srcPath + "\\GameModule\\AI\\strategies");
			AddFilesFromDir(srcPath + "\\GameModule\\Commands");
			AddFilesFromDir(srcPath + "\\GameModule\\Entities");
			AddFilesFromDir(srcPath + "\\GameModule\\Entities\\BoundaryStrategies");
			AddFilesFromDir(srcPath + "\\GameModule\\Entities\\Components");
			AddFilesFromDir(srcPath + "\\GameModule\\Handlers");

			AddFilesFromDir(srcPath + "\\MenuModule");
			AddFilesFromDir(srcPath + "\\MenuModule\\Commands");
			AddFilesFromDir(srcPath + "\\MenuModule\\MenuElements");

			AddFilesFromDir(srcPath + "\\Shared");
			AddFilesFromDir(srcPath + "\\Shared\\Structs");
			AddFilesFromDir(srcPath + "\\Shared\\Utility");
		}
		
		private void AddFilesFromDir(string dirPath) {
			string[] files = Directory.GetFiles(dirPath);

			foreach(string file in files) {
				filenames.Add(file);
			}
		}
	}
}
