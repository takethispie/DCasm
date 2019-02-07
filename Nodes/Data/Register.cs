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

        public int ToInt() {
            if(int.TryParse(Value, out int result)) return result;
            else throw new Exception("Register value parsing error"); 
        }
    }
}