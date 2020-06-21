using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Store : INode
    {
        public Store(INode baseReg, INode offsetReg, INode value)
        {
            Children = new List<INode>();
            Value = "store";
            Children.Add(value);
            Children.Add(baseReg);
            Children.Add(offsetReg);
        }

        public string Value { get; set; }
        public List<INode> Children { get; set; }
        public void Accept(IVisitor v) {
             v.Visit(this);
        }
    }
}