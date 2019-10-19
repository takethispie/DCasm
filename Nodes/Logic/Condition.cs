using System.Collections.Generic;

namespace DCasm
{
    public class Condition : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }
        public string Op { get; set; }

        public Condition(Register left, string op, Register right, INode functionCall) {
            Childrens = new List<INode>();
            Childrens.Add(left);
            Childrens.Add(right);
            Childrens.Add(functionCall);
            Op = op;
        }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}