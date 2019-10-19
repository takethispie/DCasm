using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
	public class CodeGenerator
	{
		private Scanner scanner;
		private Parser parser;
        public List<INode> rootNodes;
		public Dictionary<string, INode> Functions;
		public FileTypeEnum Type { get; set; }
		public int ErrorCount { get {
			return parser.errors.count;
		}}

		public CodeGenerator(string fileName)
		{
			scanner = new Scanner(fileName);
			parser = new Parser(scanner);
			parser.gen = this;
			rootNodes = new List<INode>();
			Functions = new Dictionary<string, INode>();
		}

		public void Compile()
		{
			IVisitor v = new Interpreter(Functions);
			rootNodes.ForEach(n => n.Accept(v));
		}


        public void Parse() => parser.Parse();

        public void ImportModule(string name) {
			if(!File.Exists(name)) throw new FileNotFoundException(name + " Not found");
			var generator = new CodeGenerator(name);
			generator.Parse();
			var moduleFunctions = generator.Functions;
			foreach(KeyValuePair<string, INode> item in moduleFunctions) {
				Functions.Add(name + "." + item.Key, item.Value);
			}
			Console.WriteLine("imported " + name);
		}
	}
}
