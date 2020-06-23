using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Div : IArithmeticNode
    {
        public INode Left { get; set; }
        public INode Right { get; set; }
        public INode Destination { get; set; }
        public string Value { get; set; }
        
        public Div(bool isImmediate)
        {
            Value = isImmediate ? "divi" : "div";
        }

    }
}