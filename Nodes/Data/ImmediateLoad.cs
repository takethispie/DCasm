using System.Collections.Generic;

namespace DCasm
{
    public class ImmediateLoad : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public bool Upper { get; set; }

        public ImmediateLoad(bool upper) {
            Value = "li";
            Childrens = new List<INode>();
            Upper = upper;
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}