using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Move : INode
    {
        public Move(INode source, INode destination)
        {
            Value = "mov";
            Destination = destination;
            Source = source;
        }

        public string Value { get; set; }
        public INode Destination { get; set; }
        public INode Source { get; set; }

    }
}