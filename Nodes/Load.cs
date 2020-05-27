using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Load : INode
    {
        public Load(INode dest, INode baseReg, INode offset)
        {
            Value = "lw";
            Children = new List<INode>();
            Children.Add(dest);
            Children.Add(baseReg);
            Children.Add(offset);
        }

        public string Value { get; set; }
        public List<INode> Children { get; set; }

        public void Accept(IVisitor v) => v.Visit(this);
    }
}