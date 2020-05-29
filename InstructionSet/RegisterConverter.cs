using System;

namespace DCasm.InstructionSet
{
    public static class RegisterConverter
    {
        public static string RegisterToBinary(INode reg) {
            var regNum = Utils.GetRegisterIndex(reg);
            return Utils.HexStringTobinaryString(Utils.DecimalToHex(regNum)).PadLeft(5, '0');
        }
    }
}