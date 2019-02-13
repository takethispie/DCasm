using System.Collections.Generic;
using System;

namespace DCasm
{
    public class Write : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}