using System;
namespace DCasm
{
	public abstract class I_ISA
	{
		string DATA_PREFIX;
		string MEMORY_PREFIX;
		string ADRESS_PREFIX;
		string IO_PREFIX;
		string SUBROUTINE_PREFIX;
		string INTERRUPTION_PREFIX;
		string ALU_PREFIX;


		public string MOV;
		public string MOVUPPER;

		public string LW;
		public string SW;
		public string LB;

		public string JMP;
		public string JLT;
		public string JEQ;
		public string JGT;
		public string BRA;
		public string BLT;
		public string BEQ;
		public string BGT;

		public string GPI;
		public string GPO;

		public string CALL;
		public string RETURN;

		public string SINT;
		public string DINT;

		public string ADD;
		public string SUB;
		public string MUL;
		public string DIV;
		public string NEG;
		public string LSHIFT;
		public string RSHIFT;
		public string RASHIFT;
		public string RND;
	}
}
