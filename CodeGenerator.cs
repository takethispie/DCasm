using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class CodeGenerator
    {
        private readonly Parser parser;
        public Dictionary<string, INode> Functions;
        public List<INode> rootNodes;
        private readonly Scanner scanner;

        public CodeGenerator(string fileName)
        {
            scanner = new Scanner(fileName);
            parser = new Parser(scanner) {gen = this};
            rootNodes = new List<INode>();
            Functions = new Dictionary<string, INode>();
        }

        public FileTypeEnum Type { get; set; }
        public int ErrorCount => parser.errors.count;

        public void Compile()
        {
            IVisitor v = new Interpreter(Functions) { verbose = false };
            rootNodes.ForEach(n => n.Accept(v));
        }


        public void Parse() => parser.Parse();

        public void ImportModule(string name)
        {
            if (!File.Exists(name)) throw new FileNotFoundException(name + " Not found");
            var generator = new CodeGenerator(name);
            generator.Parse();
            var moduleFunctions = generator.Functions;
            foreach (var (key, value) in moduleFunctions) Functions.Add(name + "." + key, value);
            Console.WriteLine("imported " + name);
        }
    }
}