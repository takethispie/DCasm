using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Load : INode {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }
        public bool Immediate { get; set; }
        public bool FromMemory { get; set; }

        public Load() {
            Value = "load";
            Childrens = new List<INode>();
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}