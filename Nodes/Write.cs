using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm;

public class Write : INode
{
    public Write(INode outputSelection, INode value)
    {
        Value = "out";
        OutputSelection = outputSelection;
        DataValue = value;
    }

    public string Value { get; set; }
    public INode OutputSelection { get; set; }
    public INode DataValue { get; set; }
}