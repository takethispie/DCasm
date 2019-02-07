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
            throw new System.NotImplementedException();
        }

        public void Visit(Const n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Function n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Load n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Add n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Sub n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Mul n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Div n)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Register n)
        {
            throw new System.NotImplementedException();
        }
    }
}