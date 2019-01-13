using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Sub : INode
    {
        public List<INode> Childrens { get; set; }
        public string Value { get; set; }

        public Sub()
        {
            Childrens = new List<INode>();
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}