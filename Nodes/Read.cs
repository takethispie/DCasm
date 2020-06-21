using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Read : INode
    {
        public Read(INode inputSelection, INode value)
        {
            Children = new List<INode>();
            Children.Add(inputSelection);
            Children.Add(value);
            Value = "in";
        }

        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}