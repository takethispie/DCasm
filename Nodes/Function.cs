using System;
using System.Collections.Generic;
using System.IO;

namespace DCasm
{
    public class Function : Node
    {
        private string name;
        public string Name { get => name; set => name = value; }

        public Function(string name) : base()
        {
            this.name = name;
        }

        public void AddNode(Node n)
        {
            this.Childrens.Add(n);
        }

        public override void Accept(Visitor v) { v.VisitFunction(this); }
    }
}