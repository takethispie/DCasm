using System;

namespace DCasm;

public static class ArithmFactory
{
    public static INode Create(string op, INode dest, INode src1, INode src2) => src2 switch
    {
        Const v => Create(op, dest, src1, src2, true),
        Register r => Create(op, dest, src1, src2, false),
        _ => new Error("Wrong argument type")
    };

    private static INode Create(string op, INode dest, INode src1, INode src2, bool isImmediate) => op switch
    {
        "add" => new Add(isImmediate) { Destination = dest, Left = src1, Right = src2 },
        "sub" => new Sub(isImmediate) { Destination = dest, Left = src1, Right = src2 },
        "mul" => new Mul(isImmediate) { Destination = dest, Left = src1, Right = src2 },
        "div" => new Div(isImmediate) { Destination = dest, Left = src1, Right = src2 },
        "lsh" => new LeftShift(isImmediate) { Destination = dest, Left = src1, Right = src2 },
        "rsh" => new RightShift(isImmediate) { Destination = dest, Left = src1, Right = src2 },
        _ => throw new ArgumentException("this arithmetic or logic operation does not exists !")
    };
}