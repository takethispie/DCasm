using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Root : INode
    {
        public List<INode> Childrens { get; set; }
        public string Value { get; set; }

        public Root()
        {
            Childrens = new List<INode>();
        }

        public void AddNode(INode n) {
            this.Childrens.Add(n);
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}