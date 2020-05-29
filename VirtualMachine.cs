using System;
using System.Collections.Generic;

namespace DCasm.Visitors
{
    public class VirtualMachine
    {
        private bool gt, eq, lt;
        private int pc;
        private List<string> rom;
        private readonly Dictionary<int, int> ram;
        private readonly int[] registers;
        private readonly Stack<int> stack;
        public bool verbose;

        public VirtualMachine(IEnumerable<string> instructions)
        {
            registers = new int[32];
            stack = new Stack<int>();
            ram = new Dictionary<int, int>();
            verbose = true;
        }

        private string HexStringTobinaryString(string hexstring) => Convert.ToString(Convert.ToInt32(hexstring, 16), 2);

        private string DecimalToHex(int decValue) => string.Format("{0:x}", decValue);

        private string HexToDecimal(string hex) => Convert.ToInt32(hex, 16).ToString();


    }
}