using System.Collections.Generic;

namespace DCasm
{
    public interface INode
    {
         List<INode> Childrens { get; set; }
         string Value { get; set; }
         void Accept(IVisitor v);
    }
}