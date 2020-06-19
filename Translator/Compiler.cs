using System;
using System.Collections.Generic;
using DCasm.InstructionSet;

namespace DCasm.Translator
{
    public class Compiler
    {
        public List<string> Program;
        private InstructionSet isa { get; set; }
        private int pc;

        public Compiler(Dictionary<string, Function> functions, bool verbose = false) {
            Program = new List<string>();
            isa = new InstructionSet();
        }

        public void Compile(List<INode> nodes) {
            
            foreach (var node in nodes)
            {
                var res = Process(node);  
                pc += res.pcIncr;
                Program.Add(res.instruction);
            }
        }

        public (string instruction, int pcIncr) Process(INode node) => node switch
        {
            Store st => isa.Store(st),
            _ => throw new Exception("Unknown Error")
        };
    }
}