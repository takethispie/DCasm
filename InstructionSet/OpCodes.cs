using System;

namespace DCasm.InstructionSet
{
    public static class OpCodes
    {
        public static string OpToBinary(string op) => op.ToLower() switch {
            "add" => "000001",
            "sub" => "000010",
            "mul" => "000011",
            "div" => "000100",
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
            "jlt" => "110000",
            "jeq" => "110001",
            "jgt" => "110010",
            "jne" => "110011",
            "mov" => "111110",
            _ => throw new Exception("Op does not exists !")
        };
    }
}