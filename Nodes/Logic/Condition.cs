using System.Collections.Generic;

namespace DCasm
{
    public class Condition : INode
    {
        public Condition(Register left, string op, Register right, INode functionCall)
        {
            Childrens = new List<INode>();
            Childrens.Add(left);
            Childrens.Add(right);
            Childrens.Add(functionCall);
            HasElseCall = false;
            Op = op;
        }

        public Condition(Register left, string op, Register right, INode functionCall, INode elseFunctionCall)
        {
            Childrens = new List<INode>();
            Childrens.Add(left);
            Childrens.Add(right);
            Childrens.Add(functionCall);
            Childrens.Add(elseFunctionCall);
            HasElseCall = true;
            Op = op;
        }

        public Condition(Register left, string op, Register right, List<INode> thenInstructions)
        {
            Childrens = new List<INode>();
            Childrens.Add(left);
            Childrens.Add(right);
            Childrens.AddRange(thenInstructions);
            HasElseCall = false;
            Op = op;
        }

        public Condition(Register left, string op, Register right, Block thenInstructions, Block elseInstructions)
        {
            Childrens = new List<INode>();
            Childrens.Add(left);
            Childrens.Add(right);
            Childrens.Add(thenInstructions);
            Childrens.Add(elseInstructions);
            HasElseCall = true;
            Op = op;
        }

        public string Op { get; set; }

        public bool HasElseCall { get; set; }
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public void Accept(IVisitor v)
        {
            v.Visit(this);
        }
    }
}