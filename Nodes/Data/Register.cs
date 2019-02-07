using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Register : INode
    {
        public string Value { get; set; }
        public int Id { get; set; }
        public List<INode> Childrens { get; set; }

        public Register() {
            Childrens = new List<INode>();
        }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}