using System;
using System.Collections.Generic;
using DCasm.InstructionSet;

namespace DCasm.Translator
{
    public class Compiler
    {
        public Compiler(Dictionary<string, INode> functions, bool verbose = false) {
            
        }

        public string Process(INode node) => node switch
        {
            _ => throw new Exception("Unknown Error")
        };
    }
}