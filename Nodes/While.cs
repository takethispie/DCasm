using System;
using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class While : INode
    {
        public string Value { get; set; }
        public List<INode> Children { get; set; }


        public While(INode callOnConditionSuccess, Comparaison comp) => init(callOnConditionSuccess, comp);

        private List<INode> init(INode callOnConditionSuccess, Comparaison comp) => callOnConditionSuccess switch {
            Call call => new List<INode> { comp, call},
            _ => throw new ArgumentException()
        };

        public void Accept(IVisitor v) => v.Visit(this);
    }
}