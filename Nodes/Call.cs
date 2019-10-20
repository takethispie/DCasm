using System.Collections.Generic;

namespace DCasm
{
    public class Call : INode
    {
        public Call(string name)
        {
            Value = name;
        }

        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}