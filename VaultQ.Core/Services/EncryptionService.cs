using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VaultQ.Core.Interfaces;
using VaultQ.Core.Models;

namespace VaultQ.Core.Services
{
    internal class EncryptionService : IEncryptionService
    {
        private const int saltSize = 16; // 128 bits Salt
        private const int keySize = 32; // 256 bits key 
        private const int ivSize = 16; // 128-bit IV for AES
        private const int iterationNumber = 100_000; 
       
        public byte[] EncryptVault(byte[] vaultBytes, string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            byte[] iv = RandomNumberGenerator.GetBytes(ivSize);
            byte[] key = new byte[keySize];

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterationNumber, HashAlgorithmName.SHA256))
            {
                key = pbkdf2.GetBytes(keySize);
            }

            using (Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

                using(MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(vaultBytes, 0, vaultBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }

                    byte[] encryptedData = ms.ToArray();
                    byte[] result = new byte[saltSize + ivSize + encryptedData.Length];

                    Buffer.BlockCopy(salt, 0, result, 0, saltSize);
                    Buffer.BlockCopy(iv, 0, result, saltSize, ivSize);
                    Buffer.BlockCopy(encryptedData, 0, result, saltSize + ivSize, encryptedData.Length);

                    return result;

                }

            }

        }

        public byte[] DecryptVault(byte[] vaultBytes, string password)
        {
            byte[] salt = vaultBytes.Take(saltSize).ToArray();
            byte[] iv = vaultBytes.Skip(saltSize).Take(ivSize).ToArray();
            byte[] data = vaultBytes.Skip(saltSize + ivSize).ToArray();

            byte[] derivedKey;
       
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterationNumber, HashAlgorithmName.SHA256))
            {
                derivedKey = pbkdf2.GetBytes(keySize);
            }

            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(derivedKey, iv);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();
                    }

                    byte[] decryptedData = ms.ToArray();

                    return decryptedData;

                }

            }

        }



    }
}
