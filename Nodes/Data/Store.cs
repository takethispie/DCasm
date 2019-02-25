using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Store : INode{

        public string Value { get; set; }
        public List<INode> Childrens { get; set; }
        
        public Store(Register baseReg, Const offset, Register value) {
            Childrens = new List<INode>();
            Value = "sw";
            Childrens.Add(value);
            Childrens.Add(baseReg);
            Childrens.Add(offset);
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}