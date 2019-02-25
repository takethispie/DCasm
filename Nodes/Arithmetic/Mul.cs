using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Mul : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }
        
        public Mul(string op)
        {
            Childrens = new List<INode>();
            Value = op;
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}