using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
	public class CodeGenerator
	{
		public int pc;              // program counter
        public int progStart;
        INode root;
		INode Current;

		public CodeGenerator()
		{
			pc = 1; 
			progStart = -1;
			root = new Root();
		}

		public void Compile()
		{
			
		}
	}
}
