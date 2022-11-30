using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm;

public class Call : INode
{
    public Call(string name)
    {
        Value = name;
    }

    public string Value { get; set; }
}