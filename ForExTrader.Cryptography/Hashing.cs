using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ForexTrader.Cryptography
{
    public class Hashing
    {
        public static byte[] ComputeSha256(string plaintext)
        {
            var plainByteArray = Encoding.UTF8.GetBytes(plaintext);
            var sha256 = SHA256.Create();
            return sha256.ComputeHash(plainByteArray);
        }        
    }
}
