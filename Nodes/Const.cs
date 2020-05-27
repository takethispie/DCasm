using System;
using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Const : INode
    {
        public Const(string value)
        {
            Value = value;
            Children = new List<INode>();
        }

        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);


        public int ToInt()
        {
            if (int.TryParse(Value, out var result)) return result;
            throw new Exception("Register value parsing error");
        }
    }
}