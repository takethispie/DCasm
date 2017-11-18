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

		//rendre intelligent cette methode pour pre process les instruction qui lui sont passé
		public void Emit(Op op, int val)
		{
			Emit(op); Put(val);
		}

		//TODO fix problem, patching erase 5 value from a = 5;
		public void Patch(int adr, int val)
		{
			code[adr] = val;
		}

		int Next()
		{
			return code[pc++];
		}

        public void Decode()
		{
			
		}

		public void Compile()
		{
			
		}
	}
}
