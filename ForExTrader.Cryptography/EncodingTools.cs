using System;
using System.Collections.Generic;
using System.Text;

namespace ForexTrader.Cryptography
{
    public class EncodingTools
    {
        public static string Base64EncodeString(string input)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64DecodeString(string input)
        {
            var encodedBytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(encodedBytes);
        }

        public static byte[] Base64DecodeStringToByteArray(string input)
        {
            return Convert.FromBase64String(input);  
        }

        public static byte[] Base64EncodeByteArray(byte[] input)
        {
            var base64String = Convert.ToBase64String(input);
            return Encoding.UTF8.GetBytes(base64String);
        }

        public static byte[] Base64DecodeByteArray(byte[] input)
        {
            var plainTextString = Encoding.UTF8.GetString(input);
            return Convert.FromBase64String(plainTextString);
        }

        public static string ByteArrayToString(byte[] input)
        {
            return Encoding.UTF8.GetString(input);
        }

        public static byte[] StringToByteArray(string input)
        {
            return Encoding.UTF8.GetBytes(input);
        }
    }
}
