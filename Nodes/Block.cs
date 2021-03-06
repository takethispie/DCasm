using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Block : INode
    {
        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public Block() {
            Children = new List<INode>(); 
        }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}