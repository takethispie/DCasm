using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Error : INode
    {
        public Error()
        {
            Value = "Error";
        }

        public Error(string errorMessage)
        {
            Value = errorMessage;
        }

        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v)
        {
            throw new Exception(Value);
        }
    }
}