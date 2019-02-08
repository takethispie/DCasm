using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Sub : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }
        public bool Unsigned { get; set; } 

        public Sub(string op, bool unsigned)
        {
            Childrens = new List<INode>();
            Value = op;
            Unsigned = unsigned;
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}