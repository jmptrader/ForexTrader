﻿using ForexTrader.Cryptography.Interfaces;
using ForexTrader.Logging.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ForexTrader.Cryptography
{
    public class Encrypt : IEncrypt
    {
        private ILogger _logger;

        public Encrypt(ILogger logger)
        {
            _logger = logger;
        }

        public string AesEncrypt(string input, string pass, byte[] iv = null)
        {
            byte[] encrypted;
            string ivString;

            using (AesManaged aes = new AesManaged())
            {
                // Encrypt the string to an array of bytes.
                aes.Key = Hashing.ComputeSha256(pass);
                encrypted = EncryptStringToBytes_Aes(input, aes.Key, aes.IV);
                ivString = EncodingTools.Base64EncodeByteArrayToString(aes.IV);
            }

            return ivString + '|' + EncodingTools.Base64EncodeByteArrayToString(encrypted);
        }

        private byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            string ivString;
            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ivString = EncodingTools.Base64EncodeByteArrayToString(aesAlg.IV);

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                try
                {
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(plainText);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
                catch (Exception e)
                {
                    encrypted = new byte[0];
                    _logger.AddLogEntry($"Failed to encrypt settings: {e.Message}");
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }
}
