using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VaultQ.Core.Enums;
using VaultQ.Core.Interfaces;
using VaultQ.Core.Models;

namespace VaultQ.Core.Services
{
    internal class EncryptionService : IEncryptionService
    {
        public byte[] EncryptVault(byte[] vaultBytes, char[] password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(VaultHeaderSizes.SaltSize);
            byte[] iv = RandomNumberGenerator.GetBytes(VaultHeaderSizes.IVSize);
            byte[] key = DeriveKeyFromPassword(password, salt, VaultHeaderSizes.IterationNumber);
            byte[] encryptedData = Encrypt(vaultBytes, key, iv);

            byte[] result = new byte[VaultHeaderSizes.HeaderSize + encryptedData.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(iv, 0, result, salt.Length, iv.Length);
            Buffer.BlockCopy(encryptedData, 0, result, VaultHeaderSizes.HeaderSize, encryptedData.Length);


            Array.Clear(salt, 0, salt.Length);
            Array.Clear(iv, 0, iv.Length);
            Array.Clear(key, 0, key.Length);
            Array.Clear(encryptedData, 0, encryptedData.Length);

            return result;             

        }
        public byte[] DecryptVault(byte[] vaultBytes, char[] password)
        {
            byte[] salt = vaultBytes.Take(VaultHeaderSizes.SaltSize).ToArray();
            byte[] iv = vaultBytes.Skip(VaultHeaderSizes.SaltSize).Take(VaultHeaderSizes.IVSize).ToArray();

            byte[] data = vaultBytes.Skip(VaultHeaderSizes.HeaderSize).ToArray();
            byte[] derivedKey = DeriveKeyFromPassword(password, salt, VaultHeaderSizes.IterationNumber);
            byte[] decryptedVault = Decrypt(data, derivedKey, iv);

            Array.Clear(salt, 0, salt.Length);
            Array.Clear(iv, 0, iv.Length);
            Array.Clear(derivedKey, 0, derivedKey.Length);

            return decryptedVault;
        }

        // Helper Method

        private byte[] DeriveKeyFromPassword(char[] password, byte[] salt, int iterationNumber)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, salt, iterationNumber, HashAlgorithmName.SHA256))
            {
                Array.Clear(passwordBytes, 0, passwordBytes.Length);
                return pbkdf2.GetBytes(VaultHeaderSizes.KeySize);
            }
            
        }

        private byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }

        private byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();
                    }

                    return ms.ToArray();
                }

            }
        }



    }
}
