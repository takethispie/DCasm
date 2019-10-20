using System.Collections.Generic;

namespace DCasm
{
    public class Store : INode
    {
        public Store(Register baseReg, Const offset, Register value)
        {
            Childrens = new List<INode>();
            Value = "sw";
            Childrens.Add(value);
            Childrens.Add(baseReg);
            Childrens.Add(offset);
        }

        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}