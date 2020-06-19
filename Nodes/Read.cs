using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Read : INode
    {
        public Read(INode inputSelection, INode value)
        {
            Value = "in";
            InputSelection = inputSelection;
            DataValue = value;
        }

        public string Value { get; set; }
        public INode InputSelection { get; set; }
        public INode DataValue { get; set; }
    }
}