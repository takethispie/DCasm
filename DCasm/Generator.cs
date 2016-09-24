using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
	public class Generator
	{
		public string hexProgramOutput;

        public void generate(List<Block> blocks)
        {
			hexProgramOutput = "";
			try
			{
				Console.WriteLine("generating program..." + Environment.NewLine);

				//will be put in a statement -> do -b /source to not be displayed by default
				foreach (Block b in blocks)
				{
					foreach (Instruction inst in b.content)
					{
						Console.WriteLine(inst.ToString(true));
					}
				}

				Console.WriteLine(Environment.NewLine);

				hexProgramOutput += "v2.0 raw" + Environment.NewLine;
				//this creates an error -> to be fixed
				foreach (Block b in blocks)
				{
					foreach (Instruction inst in b.content)
					{
						Console.WriteLine(Utils.BinToHex(inst.ToString()));
						hexProgramOutput += Utils.BinToHex(inst.ToString()) + Environment.NewLine;
					}
				}
				File.WriteAllText("test.bin", hexProgramOutput);
			}
			catch(Exception ex)
			{ Console.WriteLine("An exception occured during the generation stage" + Environment.NewLine + ex.Message); }
        }
	}
}

