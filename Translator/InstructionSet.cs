using System.Collections.Generic;
using DCasm.InstructionSet;

namespace DCasm.Translator {

    public class InstructionSet {

        public void Store(Store n, ref List<string> program,ref int pc) {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.BaseRegister) 
            + RegisterConverter.RegisterToBinary(n.OffsetRegister) + RegisterConverter.RegisterToBinary(n.DataValue)
            + "00000000000";
            program.Add(inst);
            pc++;
        }

    }

}