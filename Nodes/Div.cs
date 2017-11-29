using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Div : Node
    {
        public Div(Node child1, Node child2) : base(child1, child2)
        {
        }

        public override void Accept(Visitor v) { v.VisitDiv(this); }
    }
}