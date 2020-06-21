using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Sub : IArithmeticNode
    {
        public Sub(string op)
        {
            Value = op;
        }

        public INode Left { get; set; }
        public INode Right { get; set; }
        public INode Destination { get; set; }

        public string Value { get; set; }
    }
}