using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
	public enum Op
	{ 
		ADD, SUB, MUL, DIV, EQU, LSS, GTR, NEG,
		LOAD, LOADG, STO, STOG, CONST,
		CALL, RET, ENTER, LEAVE, JMP, FJMP, READ, WRITE
	}

	public class CodeGenerator
	{

		string[] opcode = { "ADD  ", "SUB  ", "MUL  ", "DIV  ", "EQU  ", "LSS  ", "GTR  ", "NEG  ",
	   "LOAD ", "LOADG", "STO  ", "STOG ", "CONST", "CALL ", "RET  ", "ENTER",
	   "LEAVE", "JMP  ", "FJMP ", "READ ", "WRITE" };

		public int progStart;   // address of first instruction of main program
		public int pc;              // program counter
		List<int> code = new List<int>();

		public CodeGenerator()
		{
			pc = 1; progStart = -1;
		}

		//----- code generation methods -----

		public void Put(int x)
		{
			code.Add(x);
			pc++;
		}

		public void Emit(Op op)
		{
			Put((int)op);
		}

		public void Emit(Op op, int val)
		{
			Emit(op); Put(val);
		}

		public void Patch(int adr, int val)
		{
			code[adr] = val;
		}

		public void Decode()
		{
			pc = 0;
			while (pc < code.Count)
			{
				Op code = (Op)Next();
				Console.Write("{0,3}: {1} ", pc - 1, opcode[(int)code]);
				switch (code)
				{
					case Op.LOAD:
					case Op.LOADG:
					case Op.CONST:
					case Op.STO:
					case Op.STOG:
					case Op.CALL:
					case Op.ENTER:
					case Op.JMP:
					case Op.FJMP:
						Console.WriteLine(Next()); break;
					case Op.ADD:
					case Op.SUB:
					case Op.MUL:
					case Op.DIV:
					case Op.NEG:
					case Op.EQU:
					case Op.LSS:
					case Op.GTR:
					case Op.RET:
					case Op.LEAVE:
					case Op.READ:
					case Op.WRITE:
						Console.WriteLine(); break;
				}
			}
		}

		//----- interpreter methods -----

		int Next()
		{
			return code[pc++];
		}



		public void Compile()
		{
			pc = 0;
			while (pc < code.Count)
			{
				
			}
		}
	}
}