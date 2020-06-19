using System;
using System.Collections.Generic;
using DCasm.Visitors;

namespace DCasm
{
    public class Const : INode
    {
        public Const(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public int ToInt()
        {
            if (int.TryParse(Value, out var result)) return result;
            throw new Exception("Register value parsing error");
        }
    }
}