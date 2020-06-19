using System;
using System.Collections.Generic;
using DCasm.InstructionSet;

namespace DCasm.Translator
{
    public class Compiler
    {
        public List<string> Program { get; set; }
        private InstructionSet isa { get; set; }
        private Flattener flattener { get; set; }

        public Compiler(Dictionary<string, Function> functions, bool verbose = false) {
            Program = new List<string>();
            isa = new InstructionSet();
            flattener = new Flattener(functions);
        }

        public void Compile(List<INode> nodes) {
            
            foreach (var node in nodes)
            {
                Process(node);   
            }
        }

        public string Process(INode node) => node switch
        {
            _ => throw new Exception("Unknown Error")
        };
    }
}