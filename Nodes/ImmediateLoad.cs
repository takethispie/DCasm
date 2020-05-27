using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class ImmediateLoad : INode
    {
        public ImmediateLoad(bool upper)
        {
            Value = upper ? "lui" : "li";
            Children = new List<INode>();
            Upper = upper;
        }

        public bool Upper { get; set; }
        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}