using System;
using System.IO;

namespace DCasm
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			const string version = "0.3";
			Scanner sc;
			Parser par;
			string command = "";
			//load DCASM8 Instruction Set Architecture
			Utils.currentIsa = new DCASM8();

			Console.Write("DustCat asm " + version + Environment.NewLine);
			while (command.ToUpper () != "QUIT") 
			{
				Console.Write (">");
				command = Console.ReadLine();
				string[] cmdSplit = command.Split(' ');

			    if (cmdSplit[0].ToUpper() != "DO") continue;
			    if (File.Exists(cmdSplit[1]))
			    {
			        Block.Init();
			        sc = new Scanner(cmdSplit[1]);
			        par = new Parser(sc);
			        par.gen = new Generator();
			        Console.WriteLine("Starting compilation...");
			        par.Parse();

			    }
			    else
			        Console.WriteLine("File does not exists !");
			}
		}
	}
}
