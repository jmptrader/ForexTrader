using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ForexTrader;
using ForexTrader.Cryptography.Interfaces;

namespace ForexTrader.Cryptography
{
    public class Decrypt : IDecrypt
    {
        private ConcurrentQueue<object> _loggerQueue;

        public Decrypt(ConcurrentQueue<object> loggerQueue)
        {
            _loggerQueue = loggerQueue;
        }

        public string AesDecrypt(string input, string pass, byte[] iv = null)
        {
            string decrpyted;
            var inputIvTuple = FindIv(input);
            if (inputIvTuple == null)
            {
                _loggerQueue.Enqueue($"Failed to find required IV.");
                return string.Empty;
            }

            var foundIv = inputIvTuple.Item1;
            var decodedMessage = inputIvTuple.Item2;

            using (AesManaged aes = new AesManaged())
            {
                aes.Key = Hashing.ComputeSha256(pass);
                aes.IV = foundIv;
                // Encrypt the string to an array of bytes.
                decrpyted = DecryptStringFromBytes_Aes(decodedMessage, aes.Key, aes.IV);
            }

            return decrpyted;
        }

        private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                try
                {
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {

                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    _loggerQueue.Enqueue($"Error occured when decrypting settings message: {e.Message}");
                }
            }

            return plaintext;
        }

        private Tuple<byte[], byte[]> FindIv(string input)
        {
            var splitInput = input.Split('|');
            if (splitInput.Length != 2)
            {
                _loggerQueue.Enqueue("Failed to get IV from string provided.");
                return null;
            }

            // position 0 is IV
            return Tuple.Create(EncodingTools.Base64DecodeStringToByteArray(splitInput[0]), EncodingTools.Base64DecodeStringToByteArray(splitInput[1]));
        }
    }
}
