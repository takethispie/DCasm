using System.Collections.Generic;
using System;

namespace DCasm
{
    public class InterpreterVisitor : IVisitor
    {
        public int pc;
        public bool gt, eq, lt;
        public List<string> ram;
        public int[] registers;

        public InterpreterVisitor() {
            pc = 0;
            gt = false;
            eq = false;
            lt = false;
            registers= new int[32];
        }

        public void Visit(Root n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
        }

        public void Visit(Store n)
        {
        }

        public void Visit(Const n)
        {
        }

        public void Visit(Function n)
        {
        }

        public void Visit(Load n)
        {
        }

        public void Visit(Add n)
        {
            
        }

        public void Visit(Sub n)
        {
        }

        public void Visit(Mul n)
        {
        }

        public void Visit(Div n)
        {
        }

        public void Visit(Register n)
        {
        }

        public void Visit(ImmediateLoad n)
        {
            var reg = n.Childrens[0].Value.Remove(0,1);
            var correctReg = int.TryParse(reg, out int regNumber);
            var correctValue = int.TryParse(n.Childrens[1].Value, out int value);
            if(correctReg && correctValue)  registers[regNumber] = value;
            else throw new Exception("cannot parse immediate load parameters !");
        }
    }
}