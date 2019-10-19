using System.Collections.Generic;

namespace DCasm
{
    public class Module : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}