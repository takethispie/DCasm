using System.Collections.Generic;

namespace DCasm
{
    public class ImmediateLoad : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public ImmediateLoad() {
            Value = "li";
            Childrens = new List<INode>();
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}