using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Call : INode
    {
        public Call(string name)
        {
            Value = name;
        }

        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}