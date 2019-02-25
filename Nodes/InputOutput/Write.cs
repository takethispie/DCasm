using System.Collections.Generic;
using System;

namespace DCasm
{
    public class Write : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public Write(Register outputSelection, INode value) {
            Childrens = new List<INode>();
            Childrens.Add(outputSelection);
            Childrens.Add(value);
            Value = "out";
        }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}