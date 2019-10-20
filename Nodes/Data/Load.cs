using System.Collections.Generic;

namespace DCasm
{
    public class Load : INode
    {
        public Load(Register dest, Register baseReg, Const offset)
        {
            Value = "lw";
            Childrens = new List<INode>();
            Childrens.Add(dest);
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