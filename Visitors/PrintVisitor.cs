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
            n.Childrens.ForEach(x => Console.Write(x.Value + " "));
            Console.WriteLine("");

        }

        public void Visit(Const n)
        {
            Console.WriteLine(n.Value);
        }

        public void Visit(Store n)
        {
            n.Childrens.ForEach(x => Console.Write(x.Value + " "));
            Console.WriteLine("");
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
            n.Childrens.ForEach(x => Console.Write(x.Value + " "));
            Console.WriteLine("");
        }

        public void Visit(Mul n)
        {
            n.Childrens.ForEach(x => Console.Write(x.Value + " "));
            Console.WriteLine("");
        }

        public void Visit(Div n)
        {
            n.Childrens.ForEach(x => Console.Write(x.Value + " "));
            Console.WriteLine("");
        }

        public void Visit(Register n) {

        }
    }
}