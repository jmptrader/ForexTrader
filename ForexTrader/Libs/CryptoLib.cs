using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Concurrent;

namespace ForEXTrader
{
    public class CryptoLib
    {
        private static ConcurrentQueue<object> _loggerQueue;
        public CryptoLib(ConcurrentQueue<object> loggerQueue)
        {
            _loggerQueue = loggerQueue;
        }

        public static string AesEncrypt(string input, string pass, byte[] iv = null)
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

                ivString = Base64EncodeString(aes.IV.ToString());
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

        public static string AesDecrypt(string input, string pass, byte[] iv = null)
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
                    aes.IV = Base64DecodeByteArray(inputIvTuple.Item2);
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

        private static Tuple<string, byte[]> FindIv(string input)
        {
            var splitInput = input.Split('|');
            if(splitInput.Length != 2)
            {
                _loggerQueue.Enqueue("Failed to get IV from string provided.");                
                // Still try return something further down.
            }

            // position 0 is IV
            return Tuple.Create(splitInput[0], Encoding.UTF8.GetBytes(splitInput[1]));
        }
    }
}