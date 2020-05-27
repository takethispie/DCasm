using System;
using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class While : INode
    {
        public string Value { get; set; }
        public List<INode> Children { get; set; }


        public While(INode block, Comparaison comp) {
            Children =  new List<INode> { comp, block};
        }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}