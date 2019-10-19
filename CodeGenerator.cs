using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
	public class CodeGenerator
	{
        public List<INode> rootNodes;
		public Dictionary<string, INode> Functions;

		public CodeGenerator()
		{
			rootNodes = new List<INode>();
			Functions = new Dictionary<string, INode>();
		}

		public void Compile()
		{
			IVisitor v = new Interpreter(Functions);
			rootNodes.ForEach(n => n.Accept(v));
		}
	}
}
