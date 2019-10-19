using System.Collections.Generic;

namespace DCasm
{
    public class Condition : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }
        public string Op { get; set; }

        public bool HasElseCall { get; set; }

        public Condition(Register left, string op, Register right, INode functionCall) {
            Childrens = new List<INode>();
            Childrens.Add(left);
            Childrens.Add(right);
            Childrens.Add(functionCall);
            HasElseCall = false;
            Op = op;
        }

        public Condition(Register left, string op, Register right, INode functionCall, INode elseFunctionCall) {
            Childrens = new List<INode>();
            Childrens.Add(left);
            Childrens.Add(right);
            Childrens.Add(functionCall);
            Childrens.Add(elseFunctionCall);
            HasElseCall = true;
            Op = op;
        }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}