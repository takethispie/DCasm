using System;
using System.Collections.Generic;

namespace DCasm
{
    public class Error : INode
    {
        public List<INode> Childrens { get; set; }
        public string Value { get; set; }
        public void Accept(IVisitor v)
        {
            throw new Exception("Error");
        }
    }
}