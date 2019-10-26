using System;
using System.Collections.Generic;

namespace DCasm.Visitors
{
    public class VirtualMachine : IVisitor
    {
        public Dictionary<string, INode> Functions;
        private bool gt, eq, lt;
        private int pc;
        private readonly Dictionary<int, int> ram;
        private readonly int[] registers;
        private readonly Stack<int> stack;

        public bool verbose;

        public VirtualMachine(Dictionary<string, INode> functions)
        {
            registers = new int[32];
            stack = new Stack<int>();
            ram = new Dictionary<int, int>();
            Functions = functions;
            verbose = true;
        }
        
        public void Visit(Store n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Const n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Function n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Call n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Load n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Add n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Sub n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Mul n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Div n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Register n)
        {
            throw new NotImplementedException();
        }

        public void Visit(ImmediateLoad n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Read n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Write n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Move n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Condition n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Block n)
        {
            throw new NotImplementedException();
        }


        public void Visit(While @while) {
            throw new NotImplementedException();
        }
    }
}