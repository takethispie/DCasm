using System.Collections.Generic;
using DCasm.InstructionSet;

namespace DCasm.Translator {

    public class InstructionSet {

        public void Store(DCasm.Store n, ref List<string> program,ref int pc) {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.Children[1]) 
            + RegisterConverter.RegisterToBinary(n.Children[0]) + RegisterConverter.RegisterToBinary(n.Children[2])
            + "00000000000";
            program.Add(inst);
            pc++;
        }

    }

}