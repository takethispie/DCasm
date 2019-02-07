using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Sub : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public Sub()
        {
            Childrens = new List<INode>();
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}