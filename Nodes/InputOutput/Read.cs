using System.Collections.Generic;
using System;

namespace DCasm
{
    public class Read : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public Read(Register outputSelection, INode value) {
            Childrens = new List<INode>();
            Childrens.Add(outputSelection);
            Childrens.Add(value);
            Value = "in";
        }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}