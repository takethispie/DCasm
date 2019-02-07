using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Register : INode
    {
        public string Value { get; set; }
        public int Id { get; set; }
        public List<INode> Childrens { get; set; }

        public Register(int id, string value) {
            if(int.TryParse(value, out int parsedVal)) Value = value;
            else throw new Exception("Incorrect Integer as value in register ctor");
            Id = id;
        }

        public void Accept(IVisitor v)
        {
            //v.Visit(this);
        }
    }
}