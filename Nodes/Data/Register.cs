using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Register : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }
        public Register() => Childrens = new List<INode>();
        public void Accept(IVisitor v) =>  v.Visit(this);

        public int ToInt() {
            string numOnly = Value.Remove(0, 1);
            if(int.TryParse(numOnly, out int result)) return result;
            else throw new Exception("Register value parsing error"); 
        }
    }
}