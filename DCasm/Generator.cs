using System;
using System.Collections.Generic;

namespace DCasm
{
	public class Generator
	{
		public Generator () 
        {
        }

        public void generate(List<Block> blocks)
        {
            Console.WriteLine("generating program..." + Environment.NewLine);
            foreach (Block b in blocks)
            {
                foreach(Instruction inst in b.content)
                {
                    Console.WriteLine(inst.ToString());
                }
            }
        }
	}
}

