using System.Collections.Generic;

namespace DCasm
{
    public class Call : INode 
    {
        public string Value { get; set; }
        public List<INode> Childrens { get; set; }

        public Call(string name)
        {
            this.Value = name;
        }
        
        public void Accept(IVisitor v)
        {
        }
    }
}