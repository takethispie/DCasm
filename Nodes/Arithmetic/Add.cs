using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DCasm
{
    public class Add : INode {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }
        public bool Unsigned { get; set; }

        public Add(string op, bool unsigned) {
            Childrens = new List<INode>();
            Value = op;
            Unsigned = unsigned;
        }

        public void Accept(IVisitor v) => v.Visit(this); 

        public void Reduce() {
            string op = Value;
            if(Childrens.Count < 3) throw new Exception("missing args ! only" + Childrens.Count + " arguments specified");
            
        }
    }
}