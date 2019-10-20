using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Const : INode
    {
        public Const(string value)
        {
            Value = value;
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
            if (int.TryParse(Value, out var result)) return result;
            throw new Exception("Register value parsing error");
        }
    }
}