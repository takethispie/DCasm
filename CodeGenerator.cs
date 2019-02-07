using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
	public class CodeGenerator
	{
		public int pc;              // program counter
        public int progStart;
        public INode treeRoot;
		public INode Current;

		public CodeGenerator()
		{
			pc = 1; 
			progStart = -1;
			treeRoot = new Root();
		}

		public void Compile()
		{
			PrintVisitor v = new PrintVisitor();
			treeRoot.Childrens.ForEach(x => x.Accept(v));
		}
	}
}
