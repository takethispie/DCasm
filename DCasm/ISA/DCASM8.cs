using System;
namespace DCasm
{
	public class DCASM8 : I_ISA
    {
		public static string DATA_PREFIX = "0000";
		public static string MEMORY_PREFIX = "0001";
		public static string ADRESS_PREFIX = "0010";
		public static string IO_PREFIX = "0100";
		public static string SUBROUTINE_PREFIX = "1000";
		public static string INTERRUPTION_PREFIX = "1001";


		public DCASM8() { 
			DATA_PREFIX = "0000";
			MEMORY_PREFIX = "0001";
			ADRESS_PREFIX = "0010";
			IO_PREFIX = "0100";
			SUBROUTINE_PREFIX = "1000";
			INTERRUPTION_PREFIX = "1001";


			MOV = DATA_PREFIX + "0010";
			MOVUPPER = DATA_PREFIX + "0100";

			LW = MEMORY_PREFIX + "0001";
			SW = MEMORY_PREFIX + "0010";
			LB = MEMORY_PREFIX + "0100";

			JMP = ADRESS_PREFIX + "0001";
			JLT = ADRESS_PREFIX + "0010";
			JEQ = ADRESS_PREFIX + "0100";
			JGT = ADRESS_PREFIX + "1000";
			BRA = ADRESS_PREFIX + "1001";
			BLT = ADRESS_PREFIX + "1010";
			BEQ = ADRESS_PREFIX + "1100";
			BGT = ADRESS_PREFIX + "1101";

			GPI = IO_PREFIX + "0001";
			GPO = IO_PREFIX + "0010";

			CALL = SUBROUTINE_PREFIX + "0001";
			RETURN = SUBROUTINE_PREFIX + "0010";

			SINT = INTERRUPTION_PREFIX + "0001";
			DINT = INTERRUPTION_PREFIX + "0010";
		}
    }
}
