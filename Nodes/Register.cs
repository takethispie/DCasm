using System;
using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm;

public class Register : INode
{
    public Register()
    {
    }

    public string Value { get; set; }

    public int ToInt()
    {
        var numOnly = Value.Remove(0, 1);
        if (int.TryParse(numOnly, out var result)) return result;
        throw new Exception("Register value parsing error");
    }
}