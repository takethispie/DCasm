using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Condition : INode
    {
        public bool HasElseCall { get; set; }
        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public Condition(Comparaison comp, INode thenInstructions)
        {
            Children = new List<INode> {comp};
            Children.Add(thenInstructions);
            HasElseCall = false;
        }

        public Condition(Comparaison comp, INode thenInstructions, INode elseInstructions)
        {
            Children = new List<INode> {comp, thenInstructions, elseInstructions};
            HasElseCall = true;
        }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}