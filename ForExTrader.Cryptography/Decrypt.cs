using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ForexTrader;

namespace ForexTrader.Cryptography
{
    public class Decrypt
    {
        private ConcurrentQueue<object> _loggerQueue;
        public Decrypt(ConcurrentQueue<object> loggerQueue)
        {
            _loggerQueue = loggerQueue;
        }

        public string AesDecrypt(string input, string pass, byte[] iv = null)
        {
            // Check arguments.
            if (input == null || input.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (pass == null || pass.Length <= 0)
                throw new ArgumentNullException("Key");

            byte[] decryptedByteArray;
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(pass);
                if (iv == null || iv.Length <= 0)
                {
                    // Find the IV
                    var inputIvTuple = FindIv(input);
                    input = inputIvTuple.Item1;
                    aes.IV = Base64Tools.DecodeByteArray(inputIvTuple.Item2);
                }
                else
                {
                    aes.IV = iv;
                }
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var msDecrypt = new MemoryStream())
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        using (var swDecrypt = new StreamWriter(csDecrypt))
                        {
                            // Write all data to the stream.
                            swDecrypt.Write(input);
                        }
                        decryptedByteArray = msDecrypt.ToArray();
                    }
                }

            }
            return decryptedByteArray.ToString();
        }

        private Tuple<string, byte[]> FindIv(string input)
        {
            var splitInput = input.Split('|');
            if (splitInput.Length != 2)
            {
                _loggerQueue.Enqueue("Failed to get IV from string provided.");
                // Still try return something further down.
            }

            // position 0 is IV
            return Tuple.Create(splitInput[0], Encoding.UTF8.GetBytes(splitInput[1]));
        }
    }
}
