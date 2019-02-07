using System;
using System.Collections.Generic;

namespace DCasm
{
    public class PrintVisitor : IVisitor
    {
        public PrintVisitor()
        {
        }

        public void Visit(Add n)
        {
            
        }

        public void Visit(Const n)
        {
            Console.WriteLine(n.Value);
        }

        public void Visit(Store n)
        {
            
        }

        public void Visit(Root n)
        {
        }

        public void Visit(Function n)
        {
        }

        public void Visit(Load n)
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
    }
}