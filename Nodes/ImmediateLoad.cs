using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class ImmediateLoad : INode
    {
        public ImmediateLoad(bool upper)
        {
            Value = upper ? "setu" : "set";
            Children = new List<INode>();
            Upper = upper;
        }
        
        public ImmediateLoad(bool upper, string dest, string value)
        {
            Value = upper ? "setu" : "set";
            Children = new List<INode> {
                new Register { Value = dest},
                new Const(value)
            };
            Upper = upper;
        }

        public bool Upper { get; set; }
        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}