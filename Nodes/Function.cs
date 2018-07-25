using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Function : INode
    {
        public List<INode> Childrens { get; set; }
        public string Value { get; set; }

        public Function(string name)
        {
            Value = name;
            Childrens = new List<INode>();
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}