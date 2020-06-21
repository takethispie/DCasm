using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Write : INode
    {
        public Write(INode outputSelection, INode value)
        {
            Children = new List<INode>();
            Children.Add(outputSelection);
            Children.Add(value);
            Value = "out";
        }

        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}