using System;
using System.IO;

namespace DCasm
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.Interpret("test");
        }

        static void Interpret(string filePath)
        {
            if (File.Exists(filePath))
            {
                var gen = new CodeGenerator(filePath);
                gen.Parse();
                if (gen.ErrorCount == 0) gen.Compile();
            }
            else Console.WriteLine("File does not exists !");
        }

        static void RunCli()
        {
            const string version = "0.5";
            string command = "";
            //load DCASM8 Instruction Set Architecture

            Console.Write("DustCat asm " + version + Environment.NewLine);
            while (command.ToUpper() != "QUIT")
            {
                Console.Write(">");
                command = Console.ReadLine();
                string[] cmdSplit = command.Split(' ');
                if (cmdSplit.Length < 2) throw new Exception("missing command argument !");
                if (cmdSplit[0].ToUpper() != "DO") continue;
                if (File.Exists(cmdSplit[1]))
                {
                    var gen = new CodeGenerator(cmdSplit[1]);
                    gen.Parse();
                    if (gen.ErrorCount == 0) gen.Compile();
                }
                else Console.WriteLine("File does not exists !");
            }
        }
    }
}
