using System;
using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Add : INode
    {
        public Add(string op)
        {
            Children = new List<INode>();
            Value = op;
        }

        public bool Unsigned { get; set; }
        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);

        public void Reduce()
        {
            var op = Value;
            if (Children.Count < 3)
                throw new Exception("missing args ! only" + Children.Count + " arguments specified");
        }
    }
}