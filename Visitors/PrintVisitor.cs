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
            Console.Write(n.Value + ":");
            n.Childrens.ForEach(x => Console.Write(x.Value + " "));
            Console.WriteLine("");

        }

        public void Visit(Const n)
        {
            Console.Write(n.Value + ":");
        }

        public void Visit(Store n)
        {
            Console.Write(n.Value + ":");
            n.Childrens.ForEach(x => Console.Write(x.Value + " "));
            Console.WriteLine("");
        }

        public void Visit(Root n)
        {
            n.Childrens.ForEach(x => x.Accept(this));
        }

        public void Visit(Function n)
        {
            Console.Write(n.Value + ":");
        }

        public void Visit(Load n)
        {
            Console.Write(n.Value + ":");
        }

        public void Visit(Sub n)
        {
            Console.Write(n.Value + ":");
            n.Childrens.ForEach(x => Console.Write(x.Value + " "));
            Console.WriteLine("");
        }

        public void Visit(Mul n)
        {
            Console.Write(n.Value + ":");
            n.Childrens.ForEach(x => Console.Write(x.Value + " "));
            Console.WriteLine("");
        }

        public void Visit(Div n)
        {
            Console.Write(n.Value + ":");
            n.Childrens.ForEach(x => Console.Write(x.Value + " "));
            Console.WriteLine("");
        }

        public void Visit(Register n) {
            Console.Write(n.Value + ":");
        }
    }
}