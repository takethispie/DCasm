using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Printer : IVisitor
    {
        public Printer()
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

        public void Visit(Function n)
        {
            Console.Write(n.Value + ":");
        }

        public void Visit(Call n) {
            
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

        public void Visit(ImmediateLoad n)
        {
            Console.Write("li ");
            Console.Write(n.Childrens[0].Value + " ");
            Console.WriteLine(n.Childrens[1].Value);
        }

        public void Visit(Read n)
        {
            Console.Write("in ");
            Console.WriteLine("");
        }

        public void Visit(Write n)
        {
            Console.Write("out ");
            Console.WriteLine("");
        }
        
        public void Visit(Move n) {

        }

        public void Visit(Condition n)
        {
        }

        public void Visit(Module n)
        {
            Console.WriteLine("Importe module " + n.Value);
        }
    }
}