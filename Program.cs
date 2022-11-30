using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm;

internal class Program
{
    private static void Main(string[] args)
    {
        Compile("test");
    }

    private static void Interpret(string filePath)
    {
        if (File.Exists(filePath))
        {
            var gen = new CodeGenerator(filePath);
            gen.Parse();
            if (gen.ErrorCount == 0) gen.Interpret();
        }
        else
        {
            Console.WriteLine("File does not exists !");
        }
    }

    private static void Compile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var gen = new CodeGenerator(filePath);
            gen.Parse();
            if (gen.ErrorCount != 0) return;
            var result = gen.Compile();
            var resultWithHeader = new List<String> {"v2.0 raw"};
            resultWithHeader.AddRange(result);
            File.WriteAllLines(filePath + ".hex", resultWithHeader);
        }
        else
        {
            Console.WriteLine("File does not exists !");
        }
    }

    private static void RunCli()
    {
        const string version = "0.5";
        var command = "";
        //load DCASM8 Instruction Set Architecture

        Console.Write("DustCat asm " + version + Environment.NewLine);
        while (command.ToUpper() != "QUIT")
        {
            Console.Write(">");
            command = Console.ReadLine();
            var cmdSplit = command.Split(' ');
            if (cmdSplit.Length < 2) throw new Exception("missing command argument !");
            if (cmdSplit[0].ToUpper() != "DO") continue;
            if (File.Exists(cmdSplit[1]))
            {
                var gen = new CodeGenerator(cmdSplit[1]);
                gen.Parse();
                if(gen.ErrorCount == 0) throw new Exception("Error during compilation");
                if(cmdSplit.Length > 2 && cmdSplit[2] == "-i") {
                    gen.Interpret();
                } else gen.Compile();
                
            }
            else
            {
                Console.WriteLine("File does not exists !");
            }
        }
    }
}