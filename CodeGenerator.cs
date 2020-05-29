using System;
using System.Collections.Generic;
using System.IO;
using DCasm.Visitors;

namespace DCasm
{
    public class CodeGenerator
    {
        private readonly Parser parser;
        public Dictionary<string, INode> Functions;
        public List<INode> RootNodes;
        private readonly Scanner scanner;

        public CodeGenerator(string fileName)
        {
            scanner = new Scanner(fileName);
            parser = new Parser(scanner) {gen = this};
            RootNodes = new List<INode>();
            Functions = new Dictionary<string, INode>();
        }

        public CodeGenerator(Stream stream)
        {
            scanner = new Scanner(stream);
            parser = new Parser(scanner) {gen = this};
            RootNodes = new List<INode>();
            Functions = new Dictionary<string, INode>();
        }

        public FileTypeEnum Type { get; set; }
        public int ErrorCount => parser.errors.count;

        public IEnumerable<string> Compile() {
            var v = new Compiler(Functions, false, true);
            RootNodes.ForEach(n => n.Accept(v));
            var hexProgram = new List<string>();
            foreach (var binary in v.Program) {
                var hex = Utils.BinaryToHex(binary);
                if(true) Console.WriteLine(hex);
                hexProgram.Add(hex);
            }

            return hexProgram;
        }

        public void Interpret() {
            IVisitor v = new Interpreter(Functions) { verbose = true };
            RootNodes.ForEach(n => n.Accept(v));
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