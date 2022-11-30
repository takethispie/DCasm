using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm;

public class ImmediateLoad : INode
{
    public ImmediateLoad(bool upper)
    {
        Value = upper ? "setu" : "set";
        Upper = upper;
    }
    
    public ImmediateLoad(bool upper, string dest, string value)
    {
        Value = upper ? "setu" : "set";
        Destination = new Register { Value = dest};
        DataValue = new Const(value, false);
        Upper = upper;
    }

    public bool Upper { get; set; }
    public string Value { get; set; }
    public INode Destination {get; set;}
    public INode DataValue { get; set; }

}