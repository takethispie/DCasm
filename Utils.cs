using System;

namespace DCasm
{
    public class Utils
    {
        public static string HexStringTobinaryString(string hexstring) => Convert.ToString(Convert.ToInt32(hexstring, 16), 2);

        public static string DecimalToHex(int decValue) => string.Format("{0:x}", decValue);

        public static string HexToDecimal(string hex) => Convert.ToInt32(hex, 16).ToString();

        public static string DecimalToBinary(int decValue) => HexStringTobinaryString(DecimalToHex(decValue));

        public static int GetRegisterIndex(INode n)
        {
            var reg = n.Value.Remove(0, 1);
            var correctReg = int.TryParse(reg, out var regNumber);
            if (correctReg) return regNumber;
            throw new Exception("cannot parse register number !");
        }
    }
}