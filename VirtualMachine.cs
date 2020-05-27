using System;
using System.Collections.Generic;

namespace DCasm.Visitors
{
    public class VirtualMachine
    {
        private bool gt, eq, lt;
        private int pc;
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


    }
}