using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Move : INode
    {
        public Move(Register source, Register destination)
        {
            Children = new List<INode>();
            Value = "mov";
            Children.Add(destination);
            Children.Add(source);
        }

        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}