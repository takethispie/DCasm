using System;

namespace DCasm.InstructionSet;

public static class OpCodes
{
    public static string OpToBinary(string op) => op.ToLower() switch {
        "add" => "000001",
        "sub" => "000010",
        "mul" => "000011",
        "div" => "000100",
        "lsh" => "000101",
        "rsh" => "000110",
        "lshi" => "001101",
        "rshi" => "001110",
        "addi" => "001001",
        "subi" => "001010",
        "muli" => "001011",
        "divi" => "001100",
        "store" => "101000",
        "load" => "101001",
        "setu" => "101010",
        "set" => "101011",
        "in" => "101100",
        "out" => "101101",
        "call" => "101110",
        "ret" => "101111",
        "comp" => "110001",
        "jlt" => "110010",
        "jeq" => "110011",
        "jgt" => "110100",
        "jne" => "110101",
        "jmp" => "110110",
        "jle" => "110111",
        "jge" => "111000",
        "mov" => "111100",
        _ => throw new Exception("Op does not exists !")
    };
}