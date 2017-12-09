using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Load : Node{
        public Load(Node child1, Node child2) : base(child1, child2)
        {
        }

        public override void Accept(Visitor v) { v.VisitLoad(this); }
    }
}