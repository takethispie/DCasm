using System;
using System.Collections.Generic;

namespace DCasm
{
	public class Generator
	{
        public void generate(List<Block> blocks)
        {
            Console.WriteLine("generating program..." + Environment.NewLine);

            //will be put in a statement -> do -b /source to not be displayed by default
            foreach (Block b in blocks)
            {
                foreach(Instruction inst in b.content)
                {
                    Console.WriteLine(inst.ToString(true));
                }
            }

            Console.Write(Environment.NewLine);

            foreach (Block b in blocks)
            {
                foreach(Instruction inst in b.content)
                {
                    Console.WriteLine(Utils.BinToHex(inst.ToString()));
                }
            }


        }
	}
}

