using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Add : Node{
        public Add(Node child1, Node child2) : base(child1, child2)
        {
        }

        public override void Accept(Visitor v) { v.VisitAdd(this); }
    }
}