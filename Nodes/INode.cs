using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public interface INode
    {
        string Value { get; set; }
        List<INode> Children { get; set; }
        void Accept(IVisitor v);
    }
}