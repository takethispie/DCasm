using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Add : INode
    {
        public Add(string op)
        {
            Childrens = new List<INode>();
            Value = op;
        }

        public bool Unsigned { get; set; }
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }

        public void Reduce()
        {
            var op = Value;
            if (Childrens.Count < 3)
                throw new Exception("missing args ! only" + Childrens.Count + " arguments specified");
        }
    }
}