using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Cryptography
    {
        private static string keyLocation = "../../../SecretKey.txt";

        public static string KeyLocation { get => keyLocation; }

        // Encrypts sent data using AES in CBC mode
        public static byte[] EncryptData(string plainText, string secretKey)
        {
            byte[] encrypted; // Ecnrypted data
            byte[] IV;        // Base vector for the first XOR iteration

            // Encrypting sent string
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(secretKey); // SecretKey to bytes
                aesAlg.GenerateIV(); // Generates the base vector for the first XOR iteration
                IV = aesAlg.IV;

                aesAlg.Mode = CipherMode.CBC; // Enables CBC mode for AES

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV); // Generates the encryptor

                // Stream used for encryption, mem is altered by crypto by encryptor value
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Makes a combined array of encrypted data + IV base vector for first XOR iteraton
            // Data has to have its IV vector in front as it is later used for decrypting
            var combinedIvCt = new byte[IV.Length + encrypted.Length];
            Array.Copy(IV, 0, combinedIvCt, 0, IV.Length);
            Array.Copy(encrypted, 0, combinedIvCt, IV.Length, encrypted.Length);

            return combinedIvCt;
        }

        // Used for decrypting sent data using AES in CBC mode
        public static string DecryptData(byte[] inputData, string secretKey)
        {
            string plainText = string.Empty;

            //Decrypting sent data block
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(secretKey); // SecretKey to bytes
                byte[] IV = new byte[aesAlg.BlockSize / 8]; // An array for storing the base IV vector used in the first XOR iteration
                byte[] cipherText = new byte[inputData.Length - IV.Length]; // An array for storing the data itself

                Array.Copy(inputData, IV, IV.Length); // 16bytes/128bits, thats where we stored our IV base vector for first XOR iteraton
                Array.Copy(inputData, IV.Length, cipherText, 0, cipherText.Length); // Taking the rest of bits which represent the data itself

                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC; // Enables CBC mode for AES

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV); // Generates the decryptor

                // Stream used for decryption, mem is altered by crypto by decryptor value
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }

                return plainText;
            }
        }

        public static string GenerateKey()
        {
            SymmetricAlgorithm symmAlgorithm = AesCryptoServiceProvider.Create();

            return ASCIIEncoding.ASCII.GetString(symmAlgorithm.Key);
        }


        public static void StoreKey(string secretKey, string outFile)
        {
            FileStream fOutput = new FileStream(outFile, FileMode.OpenOrCreate, FileAccess.Write);
            byte[] buffer = Encoding.ASCII.GetBytes(secretKey);

            try
            {
                fOutput.Write(buffer, 0, buffer.Length);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Storing key failed with errror -> {0}", e.Message);
            }
            finally
            {
                fOutput.Close();
            }
        }


        public static string LoadKey(string inFile)
        {
            FileStream fInput = new FileStream(inFile, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[(int)fInput.Length];

            try
            {
                fInput.Read(buffer, 0, (int)fInput.Length);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Loading key failed with error -> {0}", e.Message);
            }
            finally
            {
                fInput.Close();
            }

            return ASCIIEncoding.ASCII.GetString(buffer);
        }

    }



}
