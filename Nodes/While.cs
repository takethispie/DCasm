using System;
using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class While : INode
    {
        public string Value { get; set; }
        public List<INode> Children { get; set; }


        public While(INode callOnConditionSuccess, Comparaison comp) {
            Children = new List<INode>();
            switch (callOnConditionSuccess) {
                case Call call:
                    Children.Add(comp);
                    Children.Add(call);
                    break;
                default: throw new ArgumentException();
            }
        }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}