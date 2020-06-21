using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm {

    public class Comparaison : INode {

        public string Value { get; set; }
        public List<INode> Children { get; set; }


        public Comparaison(string op, INode left, INode right) {
            Children = new List<INode>();
            Value = op;
            Children.Add(left);
            Children.Add(right);
        }

        public void Accept(IVisitor v) => v.Visit(this);

    }

}