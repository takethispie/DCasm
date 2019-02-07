using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Div : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public Div()
        {
            Childrens = new List<INode>();
            Value = "div";
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}