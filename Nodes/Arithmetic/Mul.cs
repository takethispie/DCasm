using System.Collections.Generic;

namespace DCasm
{
    public class Mul : INode
    {
        public Mul(string op)
        {
            Childrens = new List<INode>();
            Value = op;
        }

        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}