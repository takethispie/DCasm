using System;
using System.Collections.Generic;
using System.Linq;

namespace DCasm;

public class Utils
{
    public static string HexStringTobinaryString(string hexstring) => Convert.ToString(Convert.ToInt32(hexstring, 16), 2);

    public static string DecimalToHex(int decValue) => string.Format("{0:x}", decValue);

    public static string HexToDecimal(string hex) => Convert.ToInt32(hex, 16).ToString();

    public static string DecimalToBinary(int decValue) => HexStringTobinaryString(DecimalToHex(decValue));
    
    public static string BinaryToHex(string binary) => Convert.ToInt32(binary, 2).ToString("X").ToLower();

    public static byte[] HexStringToByte(string hex) => Enumerable.Range(0, hex.Length / 2) .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)) .ToArray();

    public static byte[] HexStringToByte(IEnumerable<string> hexArray) {
        var hex = hexArray.Aggregate((string acc, string next) => acc += next);
        return Enumerable.Range(0, hex.Length / 2) .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)) .ToArray();
    }

    public static int GetRegisterIndex(INode n)
    {
        var reg = n.Value.Remove(0, 1);
        var correctReg = int.TryParse(reg, out var regNumber);
        if (correctReg) return regNumber;
        throw new Exception("cannot parse register number !");
    }
}