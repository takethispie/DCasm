using System;
using System.Collections.Generic;

namespace DCasm
{
	public class Generator
	{
        public static Dictionary<string,List<int>> labelQueue = new Dictionary<string, List<int>>();

		public Generator () { }

        public void generate(List<Instruction> program)
        {
            Console.WriteLine("generating program...");
            
        }
	}
}

