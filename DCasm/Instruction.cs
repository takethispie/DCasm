using System;
using System.Collections.Generic;

namespace DCasm
{
	public class Instruction
	{
		public string op, function, shamt;
		private List<string> parameters;
		public string machineCode;

		public string param
		{
			set
			{ parameters.Add(value); }
		}

		public Instruction(string op)
		{
			this.op = op;
			this.function = "";
			this.parameters = new List<string>();
			this.machineCode = "";
			this.shamt = "";
		}


		public List<string> getParamList()
		{
			return parameters;
		}


		public void onBlckResolution(string name, int adress)
		{
			try
			{
				if (parameters[0] == "call" && parameters[1] == name)
				{
					//if op = call
					if (op != "111110")
					{
						parameters.Clear();
						param = "0000100000";
						param = Utils.bin(Utils.hex(adress.ToString(), 4), 16);
					}
					else
					{
						parameters.Clear();
						param = "00000100000000000000000000";
					}
				}
			}
			catch (Exception ex)
			{ Console.WriteLine( "exception occured during a block resolution" + Environment.NewLine + ex.Message); }
		}


		public void onLblResolution(string name, int adress)
		{
			//reformat to binary output
			parameters.Clear();
			param = Utils.bin(Utils.hex(adress.ToString(), 4), 16);
		}


		public void create()
		{
			switch (op)
			{
				case "mov":
					op = "100001";
					break;

				case "mup":
					op = "100010";
					function = "000000";
					break;

				case "lwd":
					op = "101000";
					function = "000000";
					break;

				case "lw":
					op = "101001";
					function = "000000";
					break;

				case "swd":
					op = "101100";
					function = "000000";
					break;

				case "sw":
					op = "101101";
					function = "000000";
					break;

				case "sin":
					op = "111011";
					function = "000000";
					break;

				case "din":
					op = "111101";
					function = "000000";
					break;

				case "ein":
					op = "111100";
					function = "000000";
					break;

				case "call":
					op = "111110";
					break;

				case "ret":
					op = "111111";
					shamt = "00000";
					function = "000000";
					break;

				case "comp":
					op = "011010";
					shamt = "00000";
					function = "000000";
					break;

				case "gpo":
					op = "011011";
					param = "00000";
					shamt = "00000";
					function = "000000";
					break;

				case "gpi":
					op = "011100";
					function = "000000";
					break;

				case "jgt":
					op = "001001";
					shamt = "00000";
					function = "000000";
					break;

				case "jeq":
					op = "001010";
					shamt = "00000";
					function = "000000";
					break;

				case "jlt":
					op = "001011";
					shamt = "00000";
					function = "000000";
					break;

				case "bgt":
					op = "010001";
					shamt = "00000";
					function = "000000";
					break;

				case "beq":
					op = "010010";
					shamt = "00000";
					function = "000000";
					break;

				case "blt":
					op = "010011";
					shamt = "00000";
					function = "000000";
					break;

				case "jmp":
					op = "001111";
					shamt = "00000";
					function = "000000";
					break;

				case "bra":
					op = "010111";
					shamt = "00000";
					function = "000000";
					break;

				case "add":
					op = "000000";
					shamt = "00000";
					function = "000001";
					break;

				case "sub":
					op = "000000";
					shamt = "00000";
					function = "000002";
					break;

				case "mul":
					op = "000000";
					shamt = "00000";
					function = "000003";
					break;

				case "div":
					op = "000000";
					shamt = "00000";
					function = "000004";
					break;
			}
		}


		public void clearParams()
		{
			parameters.Clear();
		}


		public string ToString(bool showSize)
		{
			try
			{
				string strParam = "";
				foreach (string para in parameters)
				{
					strParam += para;
				}
				string result = op + strParam + shamt + function;
				return string.Format(result + " : " + result.Length);
			}
			catch (Exception ex)
			{ Console.WriteLine(ex.Message); }
			return "";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="showSize"></param>
		/// <returns></returns>
		public override string ToString()
		{
			try
			{
				string strParam = "";
				foreach (string para in parameters)
				{
					strParam += para;
				}
				string result = op + strParam + shamt + function;
				return result;
			}
			catch (Exception ex)
			{ Console.WriteLine(ex.Message); }
			return "";
        	}
	}
}
