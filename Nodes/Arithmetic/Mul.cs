using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Mul : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }
        
        public Mul()
        {
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}