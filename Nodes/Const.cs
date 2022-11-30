using System;
using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm;

public class Const : INode
{
    public Const(string value, bool isHexa)
    {
        Value = isHexa ? Convert.ToInt32( value.Remove(0, 1) , 16).ToString() : value;
    }

    public string Value { get; set; }

    public int ToInt()
    {
        if (int.TryParse(Value, out var result)) return result;
        throw new Exception("Register value parsing error");
    }
}