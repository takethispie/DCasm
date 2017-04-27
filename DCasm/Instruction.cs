using System;
using System.Collections.Generic;
using System.Linq;

namespace DCasm
{
	public class Instruction
	{
		public string op;
		public List<string> parameters;

		public Instruction(string op)
		{
			this.op = op;
			parameters = new List<string>();
		}


		/// <summary>
		/// resolve adress
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="adress">Adress.</param>
		public void onBlckResolution(string name, int adress)
		{
			try
			{
			    if (parameters[0] != "call" || parameters[1] != name) return;
			    //if op =mov then set call target adress as parameter
				if (op != Utils.currentIsa.CALL) {
			        parameters.Clear();
					parameters.Add("00001000");
					parameters.Add(Utils.bin(Utils.hex(adress.ToString(), 4), 16));
			    } else {
					//if op = call then set the right register as parameter
			        parameters.Clear();
					parameters.Add("000001000000000000000000");
			    }
			}
			catch (Exception ex)
			{ Console.WriteLine( "exception occured during a block resolution" + Environment.NewLine + ex.Message); }
		}


		public void create()
		{
		    switch (op)
		    {
		        case "mov":
					op = Utils.currentIsa.MOV;
		            break;

				case "add":
					op = Utils.currentIsa.ADD;
					break;
					
				case "sub":
					op = Utils.currentIsa.SUB;
					break;
					
		        case "mul":
					op = Utils.currentIsa.MUL;
					break;

				case "div":
					op = Utils.currentIsa.DIV;
					break;

				case "mup":
					op = Utils.currentIsa.MOVUPPER;
					break;

		        case "gpo":
					op = Utils.currentIsa.GPO;
					parameters.Add("00000000000000");
					break;

				case "gpi":
					op = Utils.currentIsa.GPI;
					break;

				case "lw":
					op = Utils.currentIsa.LW;
					break;

				case "sw":
					op = Utils.currentIsa.SW;
					break;

				case "jmp":
					op = Utils.currentIsa.JMP;
					break;

				case "jgt":
					op = Utils.currentIsa.JGT;
					break;

				case "jeq":
					op = Utils.currentIsa.JEQ;
					break;

				case "jlt":
					op = Utils.currentIsa.JLT;
					break;

				case "call":
					op = Utils.currentIsa.CALL;
					break;

				case "return":
					op = Utils.currentIsa.RETURN;
					break;

				case "sint":
					op = Utils.currentIsa.SINT;
					break;

				case "dint":
					op = Utils.currentIsa.DINT;
					break;

				case "neg":
					op = Utils.currentIsa.NEG;
					break;

				case "lshift":
					op = Utils.currentIsa.LSHIFT;
					break;

				case "rshift":
					op = Utils.currentIsa.RSHIFT;
					break;

				case "rashift":
					op = Utils.currentIsa.RASHIFT;
					break;

				case "rnd":
					op = Utils.currentIsa.RND;
					break;
		    }
		}


		public void clearParams()
		{
			parameters.Clear();
		}


		public string ToString(bool showSize)
		{
			try {
				string strParam = parameters.Aggregate("", 
				                                       (current, para) => current + para
				                                      );
			    string result = op + strParam;
				//if (showSize) { 
				//	result = string.Format(result + " : " + result.Length);
				//}
				return result;
			}
			catch (Exception ex) { 
				Console.WriteLine(ex.Message); 
			}
			return "";
		}
	}
}
