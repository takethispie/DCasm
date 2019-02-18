using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
	public class CodeGenerator
	{
        public INode treeRoot;
		public Dictionary<string, INode> Functions;

		public CodeGenerator()
		{
			treeRoot = new Root();
			Functions = new Dictionary<string, INode>();
		}

		public void Compile()
		{
			IVisitor v = new Interpreter(Functions);
			treeRoot.Accept(v);
		}
	}
}
