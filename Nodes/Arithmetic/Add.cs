using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Add : INode {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public Add() {
            Childrens = new List<INode>();
        }

        public void Accept(IVisitor v) => v.Visit(this); 
    }
}