using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.Cryptography
{
    public class Base64Tools
    {
        public static string EncodeString(string input)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string DecodeString(string input)
        {
            var encodedBytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(encodedBytes);
        }

        public static byte[] EncodeByteArray(byte[] input)
        {
            var base64String = Convert.ToBase64String(input);
            return Encoding.UTF8.GetBytes(base64String);
        }

        public static byte[] DecodeByteArray(byte[] input)
        {
            var plainTextString = Encoding.UTF8.GetString(input);
            return Convert.FromBase64String(plainTextString);
        }
    }
}
