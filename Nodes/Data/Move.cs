using System.Collections.Generic;

namespace DCasm
{
    public class Move : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public Move(Register source, Register destination) {
            Childrens = new List<INode>();
            Value = "mov";
            Childrens.Add(destination);
            Childrens.Add(source);
        }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}