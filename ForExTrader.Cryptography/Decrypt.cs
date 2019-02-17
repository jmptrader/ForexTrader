using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ForexTrader;
using ForexTrader.Cryptography.Interfaces;
using ForexTrader.Logging.Interfaces;

namespace ForexTrader.Cryptography
{
    public class Decrypt : IDecrypt
    {
        private readonly ILogger _logger;

        public Decrypt(ILogger logger)
        {
            _logger = logger;
        }

        public string AesDecrypt(string input, string pass, byte[] iv = null)
        {
            string decrpyted;
            byte[] encryptedMessage = EncodingTools.StringToByteArray(input);
            if (iv == null)
            {
                var inputIvTuple = FindIv(input);
                iv = inputIvTuple.Item1;
                encryptedMessage = inputIvTuple.Item2;

                // if still null :(
                if (iv == null || encryptedMessage == null)
                {
                    _logger.AddLogEntry($"Failed to gather required information. iv: {iv}, encryptedMessage: {encryptedMessage}");
                    return string.Empty;
                }
            }

            try
            {
                using (AesManaged aes = new AesManaged())
                {
                    aes.Key = Hashing.ComputeSha256(pass);
                    aes.IV = iv;
                    // Encrypt the string to an array of bytes.
                    decrpyted = DecryptStringFromBytes_Aes(encryptedMessage, aes.Key, aes.IV);
                }
            }
            catch (Exception e)
            {
                _logger.AddLogEntry($"Failed to decrypt message: {e}");
                return string.Empty;
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
                    _logger.AddLogEntry($"Error occured when decrypting settings message: {e.Message}");
                }
            }

            return plaintext;
        }

        private Tuple<byte[], byte[]> FindIv(string input)
        {
            var splitInput = input.Split('|');
            if (splitInput.Length != 2)
            {
                _logger.AddLogEntry("Failed to get IV from string provided.");
                return null;
            }

            // position 0 is IV
            return Tuple.Create(EncodingTools.Base64DecodeStringToByteArray(splitInput[0]), EncodingTools.Base64DecodeStringToByteArray(splitInput[1]));
        }
    }
}
