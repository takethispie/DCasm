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
		INode root;
		INode Current;
		List<INode> Nodes;

		public CodeGenerator()
		{
			pc = 1; progStart = -1;
			root = new Root();
			Nodes = new List<INode>();
		}

		//----- code generation methods -----
		public void Emit(Op op)
		{
			
		}

		//rendre intelligent cette methode pour pre process les instruction qui lui sont passé
		public void Emit(Op op, int val)
		{
			switch(op) {
				case Op.CONST:
					Current = new Const(val);
				break;

				case Op.STO:
					Console.Write("store: ");
					Current.Accept(new PrintVisitor());
				break;
			}
		}

        public void Decode()
		{
			
		}

		public void Compile()
		{
			
		}
	}
}
