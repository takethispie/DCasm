using System.Collections.Generic;

namespace DCasm
{
    public class Condition : INode
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public Condition(Register left, string op, Register right, Function inFunction) {

        }

        public void Accept(IVisitor v)
        {
        }
    }
}