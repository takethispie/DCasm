using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Return : INode
    {
        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public Return(string functionName) {
            Value = functionName;
        }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}