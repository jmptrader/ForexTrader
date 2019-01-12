using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ForexTrader.Cryptography
{
    public class Encrypt
    {
        public Encrypt()
        {
        }

        public string AesEncrpyt(string input, string pass, byte[] iv = null)
        {
            // Check arguments.
            if (input == null || input.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (pass == null || pass.Length <= 0)
                throw new ArgumentNullException("Key");

            byte[] encryptedByteArray;
            string ivString;
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(pass);
                if (iv == null || iv.Length <= 0)
                {
                    // Generate an IV
                    aes.GenerateIV();
                }
                else
                {
                    aes.IV = iv;
                }

                ivString = Base64Tools.EncodeString(aes.IV.ToString());
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(input);
                        }
                        encryptedByteArray = msEncrypt.ToArray();
                    }
                }

            }
            return ivString + '|' + encryptedByteArray.ToString();
        }
    }
}
