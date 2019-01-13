using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Const : INode {
        public List<INode> Childrens { get; set; }
        public string Value { get; set; }

        public Const(int value) {
            this.Value = value.ToString();  
            Childrens = new List<INode>();  
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}