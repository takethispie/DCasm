using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Load : INode {
        public List<INode> Childrens { get; set; }
        public string Value { get; set; }
        public Load()
        {
            Childrens = new List<INode>();
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}