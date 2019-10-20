using System.Collections.Generic;

namespace DCasm
{
    public class Read : INode
    {
        public Read(Register inputSelection, INode value)
        {
            Childrens = new List<INode>();
            Childrens.Add(inputSelection);
            Childrens.Add(value);
            Value = "in";
        }

        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}