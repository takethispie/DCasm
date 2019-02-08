using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Store : INode{

        public string Value { get; set; }
        public List<INode> Childrens { get; set; }
        
        public Store() {
            Childrens = new List<INode>();
            Value = "store";
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}