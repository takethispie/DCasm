using System.Collections.Generic;

namespace DCasm
{
    public class ImmediateLoad : INode
    {
        public ImmediateLoad(bool upper)
        {
            Value = upper ? "lui" : "li";
            Childrens = new List<INode>();
            Upper = upper;
        }

        public bool Upper { get; set; }
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}