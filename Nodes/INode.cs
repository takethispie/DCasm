using System.Collections.Generic;

namespace DCasm
{
    public interface INode
    { 
         string Value { get; set; }
         List<INode> Childrens { get; set; }
         void Accept(IVisitor v);
    }
}