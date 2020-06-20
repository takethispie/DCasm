using System;
using System.Collections.Generic;
using DCasm.InstructionSet;

namespace DCasm.Translator
{
    public class Compiler
    {
        public List<string> Program;
        private Dictionary<string, int> functionsAdress { get; set; }
        private int pc;

        public Compiler(Dictionary<string, Function> functions, bool verbose = false) {
            Program = new List<string>();
        }

        public void Compile(List<INode> nodes) {
            
            foreach (var node in nodes)
            {
                var res = Process(node);  
                pc += res.pcIncr;
                Program.AddRange(res.instruction);
            }
        }

        public (IEnumerable<string> instruction, int pcIncr) Process(INode node) => node switch
        {
            Store st => Store(st),
            _ => throw new Exception("Unknown Error")
        };

        public (IEnumerable<string> instruction, int pcIncr) Store(Store n) {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.BaseRegister) 
            + RegisterConverter.RegisterToBinary(n.OffsetRegister) + RegisterConverter.RegisterToBinary(n.DataValue)
            + "00000000000";
            return (new List<string> { inst }, 1);
        }

        public (IEnumerable<string> instruction, int pcIncr) Call (Call n)
        {
            var address = functionsAdress[n.Value];
            var setInst = OpCodes.OpToBinary("set") + "00111" + "00000" + ConstConverter.ConstantToBinary(address.ToString());
            var inst = OpCodes.OpToBinary("call") + "00000" + "00111" + ConstConverter.ConstantToBinary("0");
            return (new List<string> { setInst, inst}, 2);
        }
    }
}