using System.Collections.Generic;

namespace DCasm
{
    public class Block : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public Block() {
            Childrens = new List<INode>(); 
        }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}