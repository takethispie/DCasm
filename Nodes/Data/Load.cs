using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Load : INode {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public Load(Register dest, Register baseReg, Const offset) {
            Value = "lw";
            Childrens = new List<INode>();
            Childrens.Add(dest);
            Childrens.Add(baseReg);
            Childrens.Add(offset);
        }

        public void Accept(IVisitor v) { v.Visit(this); }
    }
}