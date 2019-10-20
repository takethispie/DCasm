using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Register : INode
    {
        public Register()
        {
            Childrens = new List<INode>();
        }

        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }

        public int ToInt()
        {
            var numOnly = Value.Remove(0, 1);
            if (int.TryParse(numOnly, out var result)) return result;
            throw new Exception("Register value parsing error");
        }
    }
}