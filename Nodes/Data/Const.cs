using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Const : INode {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public Const(string value) {
            this.Value = value;  
            Childrens = new List<INode>();
        }

        public void Accept(IVisitor v) { v.Visit(this); }

        
        public int ToInt() {
            if(int.TryParse(Value, out int result)) return result;
            else throw new Exception("Register value parsing error"); 
        }
    }
}