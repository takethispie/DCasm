using System;
using System.Collections.Generic;

namespace DCasm
{
	public class Part
	{
        //table values
		//1 = hex, 2 = reg, 3 = ident, 4 = number, 5 = string
		public int type;
		public string value;

		public Part(int t,string va)
		{
			this.type = t;
			this.value = va;
            convert();
		}

        public void convert()
        {
            switch(this.type)
            {
                case 1:
                    //remove 0x
                    this.value = this.value.Remove(0, 2);
                    this.value = Utils.bin(this.value, 16);
                    break;

                case 2:
                    //remove $
                    this.value = this.value.Remove(0, 1);
                    this.value = Utils.hex(this.value, 2);
                    this.value = Utils.bin(this.value, 5);
                    break;
            }
        }
	}


	public class Instruction
	{
		public string op;
		public List<Part> parts;
		public int index;
        public int machineCodeSize;
        public List<string> machineCode;


        public Instruction (string op)
		{
			this.op = op;
			parts = new List<Part> ();
            machineCodeSize = 1;
            index = 0;
		}


		public void addArg(int type,string value)
		{
			parts.Add (new Part (type, value));
		}


		public void create()
		{
            #if DEBUG
                Console.Write(this.op + ": ");
                foreach(Part p in this.parts)
                {
                    Console.Write(p.value + " ");
                }
                Console.Write(Environment.NewLine);
            #endif
            
		}
	}
}

