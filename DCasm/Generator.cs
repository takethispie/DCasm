using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    /// <summary>
    /// hexa generator
    /// </summary>
	public class Generator
	{
		public string hexProgramOutput;

	    /// <summary>
	    /// generate hexadecimal file from code blocks
	    /// </summary>
	    /// <param name="blocks">the program to write into a file</param>
        public void generate(List<Block> blocks)
        {
			hexProgramOutput = "";
			try
			{
				Console.WriteLine("generating program..." + Environment.NewLine);

				//will be put in a statement -> do -b /source to not be displayed by default
				foreach (var b in blocks) {
					foreach (var inst in b.content) {
						Console.WriteLine(inst.ToString(true));
					}
				}

				Console.WriteLine(Environment.NewLine);

				hexProgramOutput += "v2.0 raw" + Environment.NewLine;
				//this creates an error -> to be fixed
				foreach (var b in blocks) {
					foreach (var inst in b.content) {
						Console.WriteLine(Utils.BinToHex(inst.ToString(true)));
						hexProgramOutput += Utils.BinToHex(inst.ToString(true)) + Environment.NewLine;
					}
				}
				File.WriteAllText("test.bin", hexProgramOutput);
			}
			catch(Exception ex)
			{ Console.WriteLine("An exception occured during the generation stage" + Environment.NewLine + ex.Message); }
        }
	}
}

