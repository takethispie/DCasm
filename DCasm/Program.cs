using System;
using System.IO;

namespace DCasm
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string version = "0.2"; 
			Scanner sc;
			Parser par;
			string command = "";

			Console.Write("DustCat asm " + version + Environment.NewLine);
			while (command.ToUpper () != "QUIT") 
			{
				Console.Write (">");
				command = Console.ReadLine();
				string[] cmdSplit = command.Split(' ');

                if (cmdSplit[0].ToUpper() == "DO")
                {
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
}
