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
            throw new System.NotImplementedException();
        }

        public void Visit(Const n)
        {
            stack.Push(n.ToInt());
        }

        public void Visit(Function n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Call n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Load n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Add n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            var destReg = stack.Pop();
            registers[destReg] = src1 + src2;
        }

        public void Visit(Sub n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            var destReg = stack.Pop();
            registers[destReg] = src1 - src2;
        }

        public void Visit(Mul n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            var destReg = stack.Pop();
            registers[destReg] = src1 * src2;
        }

        public void Visit(Div n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            var destReg = stack.Pop();
            if (src2 == 0) throw new DivideByZeroException();
            registers[destReg] = src1 / src2;
        }

        public void Visit(Register n)
        {
            stack.Push(GetRegisterValue(n));
        }

        public void Visit(ImmediateLoad n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Read n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Write n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Move n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Condition n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Block n)
        {
            throw new System.NotImplementedException();
        }

        private int GetRegisterValue(INode n)
        {
            return registers[GetRegisterIndex(n)];
        }

        private int GetRegisterIndex(INode n)
        {
            var reg = n.Value.Remove(0, 1);
            if (int.TryParse(reg, out var regNumber)) return regNumber;
            throw new Exception("cannot parse register number !");
        }
    }
}