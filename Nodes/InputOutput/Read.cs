using System.Collections.Generic;
using System;

namespace DCasm
{
    public class Read : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}