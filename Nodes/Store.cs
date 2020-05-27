using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Store : INode
    {
        public Store(INode baseReg, INode offset, INode value)
        {
            Children = new List<INode>();
            Value = "sw";
            Children.Add(value);
            Children.Add(baseReg);
            Children.Add(offset);
        }

        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}