using System;

namespace DCasm
{
    public static class Utils
    {
        /// <summary>
        /// convert to Hex the specified int.
        /// </summary>
        /// <param name="str">value</param>
        /// <param name="size"> returned digit size </param>
        public static string hex(string str, int size)
        {
            int param = int.Parse(str);
            string hexValue = param.ToString("X" + size.ToString());
            return hexValue;
        }

        /// <summary>
        /// return the specified hexadecimal value in a string by 
        /// his binary equivalent in a string
        /// </summary>
        /// <param name="str">value</param>
        /// <param name="size">returned digit size</param>
        public static string bin(string str, int size)
        {
            return Convert.ToString(Convert.ToInt32(str, 16), 2).PadLeft(size, '0');
        }
    }    
}

