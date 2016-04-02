using System;
using System.Collections.Generic;

namespace DCasm
{
	public class Instruction
	{
		public string op,function,shamt;
        private List<string> parameters;
        public string machineCode;
		
        public string param
        {
            set
            { parameters.Add(value);}
        }

        public Instruction (string op)
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
            if (parameters[0] == "call" && parameters[1] == name)
            {
                parameters.Clear();
                param = Utils.bin(Utils.hex(adress.ToString(),4),16);
            }
        }


        public void onLblResolution(string name, int adress)
        {
            parameters.Clear();
            param = Utils.bin(Utils.hex(adress.ToString(),4),16);
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
                    break;

                case "lwd":
                    op = "101000";
                    break;

                case "lw":
                    op = "101001";
                    break;

                case "swd":
                    op = "101100";
                    break;

                case "sw":
                    op = "101101";
                    break;

                case "sin":
                    op = "111011";
                    break;

                case "din":
                    op = "111101";
                    break;

                case "ein":
                    op = "111100";
                    break;

                case "call":
                    op = "111110";
                    break;

                case "ret":
                    op = "111111";
                    break;

                case "comp":
                    op = "011010";
                    break;

                case "gpo":
                    op = "011011";
                    break;

                case "gpi":
                    op = "011100";
                    break;

                case "jgt":
                    op = "001001";
                    break;

                case "jeq":
                    op = "001010";
                    break;

                case "jlt":
                    op = "001011";
                    break;

                case "bgt":
                    op = "010001";
                    break;

                case "beq":
                    op = "010010";
                    break;

                case "blt":
                    op = "010011";
                    break;

                case "jmp":
                    op = "001111";
                    break;

                case "bra":
                    op = "010111";
                    break;

                case "add":
                    op = "000000";
                    break;

                case "sub":
                    op = "000000";
                    break;

                case "mul":
                    op = "000000";
                    break;

                case "div":
                    op = "000000";
                    break;
            }
		}


        public void clearParams()
        {
            parameters.Clear();
        }


        public override string ToString()
        {
            string strParam = "";
            foreach (string para in parameters)
            {
                strParam += para + " ";
            }
            return string.Format(op + " " + shamt + strParam);
        }
	}
}

