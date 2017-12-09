using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Function : Node
    {
        public Function(Node child1) : base(child1)
        {
        }

        public void AddNode(Node n)
        {
            this.childrens.Add(n);
        }

        public override void Accept(Visitor v) { v.VisitFunction(this); }
    }
}