using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
	public class CodeGenerator
	{
        public INode treeRoot;
		public Dictionary<string,INode> symbolTable;

		public CodeGenerator()
		{
			treeRoot = new Root();
			symbolTable = new Dictionary<string, INode>();
		}

		public void Compile()
		{
			IVisitor v = new Interpreter();
			treeRoot.Accept(v);
		}
	}
}
