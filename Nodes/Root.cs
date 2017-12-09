using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Root : Node
    {
        public Root() : base()
        {
        }

        public void AddNode(Node n) {
            this.childrens.Add(n);
        }

        public override void Accept(Visitor v) { v.VisitRoot(this); }
    }
}