using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Div : INode
    {
        public Div(string op)
        {
            Children = new List<INode>();
            Value = op;
        }

        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}