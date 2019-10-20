using System.Collections.Generic;

namespace DCasm
{
    public class Function : INode
    {
        public Function(string name)
        {
            Value = name;
            Childrens = new List<INode>();
        }

        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}