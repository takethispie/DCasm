namespace DCasm.InstructionSet;

public static class ConstConverter
{
    public static string ConstantToBinary(string constant) {
        var c = int.Parse(constant);
        return Utils.HexStringTobinaryString(Utils.DecimalToHex(c)).PadLeft(16, '0');
    }
}