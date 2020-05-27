using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Function : INode
    {
        public Function(string name)
        {
            Value = name;
            Children = new List<INode>();
        }

        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}