using System.Collections.Generic;
using DCasm.InstructionSet;

namespace DCasm.Translator {

    public class InstructionSet {

        public (string instruction, int pcIncr) Store(Store n) {
            var inst = OpCodes.OpToBinary(n.Value) + RegisterConverter.RegisterToBinary(n.BaseRegister) 
            + RegisterConverter.RegisterToBinary(n.OffsetRegister) + RegisterConverter.RegisterToBinary(n.DataValue)
            + "00000000000";
            return (inst, 1);
        }

    }

}