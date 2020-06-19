using System;
using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class While : INode
    {
        public string Value { get; set; }
        public List<INode> Children { get; set; }
        public Comparaison Comparaison { get; set; }
        public INode Block { get; set; }


        public While(INode block, Comparaison comp) {
            Comparaison = comp;
            Block = block;
        }

    }
}