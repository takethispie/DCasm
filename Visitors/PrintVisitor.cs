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
            if(n.Childrens.Count > 1) {
                n.Childrens[0].Accept(this);
                Console.WriteLine("+");
                n.Childrens[1].Accept(this);
            }
        }

        public void Visit(Const n)
        {
            Console.WriteLine(n.Value);
        }

        public void Visit(Store n)
        {
            if(n.Childrens.Count > 1) {
                n.Childrens[0].Accept(this);
                n.Childrens[1].Accept(this);
            }
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