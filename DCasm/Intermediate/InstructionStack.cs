using System;
using System.Collections.Generic;

public class InstructionStack
{
    public List<string> instructions;

    public InstructionStack()
    {
        instructions = new List<string>();
    }

    public void Add (string val) 
    {
        instructions.Add(val);
    }

    public void Display() 
    {
        instructions.ForEach((data)=>{ Console.WriteLine(data); });
    }
}