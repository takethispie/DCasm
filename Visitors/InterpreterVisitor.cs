using System.Collections.Generic;
using System;

namespace DCasm
{
    public class InterpreterVisitor : IVisitor
    {
        private int pc;
        private bool gt, eq, lt;
        private Dictionary<int, int> ram;
        private int[] registers;
        private Stack<int> stack;

        public InterpreterVisitor() {
            pc = 0;
            gt = false;
            eq = false;
            lt = false;
            registers = new int[32];
            stack = new Stack<int>();
            ram = new Dictionary<int, int>();
        }

        private int GetRegisterValue(INode n) {
            var index = GetRegisterIndex(n);
            return registers[index];
        }

        private int GetRegisterIndex(INode n) {
            var reg = n.Value.Remove(0,1);
            var correctReg = int.TryParse(reg, out int regNumber);
            if(correctReg)  return regNumber;
            else throw new Exception("cannot parse register number !");
        }

        public void Visit(Root n) => n.Childrens.ForEach(x => x.Accept(this));

        public void Visit(Store n)
        {
        }

        public void Visit(Const n) => stack.Push(n.ToInt());

        public void Visit(Function n)
        {
        }

        public void Visit(Load n)
        {
        }

        public void Visit(Add n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            registers[regNumber] = src1 + src2;
            Console.WriteLine(src1 + " + " + src2 + " => $" + regNumber);
            Console.WriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Sub n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            registers[regNumber] = src1 - src2;
            Console.WriteLine(src1 + " - " + src2 + " => $" + regNumber);
            Console.WriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Mul n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            registers[regNumber] = src1 * src2;
            Console.WriteLine(src1 + " * " + src2 + " => $" + regNumber);
            Console.WriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Div n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
            var src2 = stack.Pop();
            var src1 = stack.Pop();
            stack.Pop();
            var regNumber = GetRegisterIndex(n.Childrens[0]);
            if(src2 == 0) throw new DivideByZeroException();
            registers[regNumber] = src1 / src2;
            Console.WriteLine(src1 + " - " + src2 + " => $" + regNumber);
            Console.WriteLine("$" + regNumber + " = " + registers[regNumber]);
        }

        public void Visit(Register n) => stack.Push(GetRegisterValue(n));

        public void Visit(ImmediateLoad n)
        {
            var reg = n.Childrens[0].Value.Remove(0,1);
            var correctReg = int.TryParse(reg, out int regNumber);
            var correctValue = int.TryParse(n.Childrens[1].Value, out int value);
            if(correctReg && correctValue)  registers[regNumber] = value;
            else throw new Exception("cannot parse immediate load parameters !");
            Console.WriteLine("stored " + value + " in $" + regNumber);
        }

        public void Visit(Read n)
        {
        }

        public void Visit(Write n)
        {
        }

        public void Visit(Move n) {
            var source = stack.Pop();
            var destination = stack.Pop();
            registers[destination] = registers[source];
            Console.WriteLine("$" + source + "(" + registers[source] + ") => $" + destination + "(" + registers[destination] + ")");
        }
    }
}