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
            byte[] salt = RandomNumberGenerator.GetBytes(VaultHeaderInfo.SaltSize);
            byte[] iv = RandomNumberGenerator.GetBytes(VaultHeaderInfo.IVSize);
            byte[] key = DeriveKeyFromPassword(password, salt, VaultHeaderInfo.IterationNumber);
            byte[] encryptedData = Encrypt(vaultBytes, key, iv);

            byte[] encryptedCheckerBytes = Encrypt(Encoding.UTF8.GetBytes(VaultHeaderInfo.VaultChecker), key, iv);

            byte[] result = new byte[VaultHeaderInfo.HeaderSize + encryptedData.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(iv, 0, result, salt.Length, iv.Length);
            Buffer.BlockCopy(encryptedCheckerBytes, 0, result, salt.Length + iv.Length, encryptedCheckerBytes.Length);
            Buffer.BlockCopy(encryptedData, 0, result, VaultHeaderInfo.HeaderSize, encryptedData.Length);


            Array.Clear(salt, 0, salt.Length);
            Array.Clear(iv, 0, iv.Length);
            Array.Clear(key, 0, key.Length);
            Array.Clear(encryptedData, 0, encryptedData.Length);
            Array.Clear(encryptedCheckerBytes, 0, encryptedCheckerBytes.Length);

            return result;             

        }

        public byte[] DecryptVault(byte[] vaultBytes, char[] password)
        {
            byte[] salt = vaultBytes.Take(VaultHeaderInfo.SaltSize).ToArray();
            byte[] iv = vaultBytes.Skip(VaultHeaderInfo.SaltSize).Take(VaultHeaderInfo.IVSize).ToArray();

            byte[] data = vaultBytes.Skip(VaultHeaderInfo.HeaderSize).ToArray();
            byte[] derivedKey = DeriveKeyFromPassword(password, salt, VaultHeaderInfo.IterationNumber);
            byte[] decryptedVault = Decrypt(data, derivedKey, iv);

            Array.Clear(salt, 0, salt.Length);
            Array.Clear(iv, 0, iv.Length);
            Array.Clear(derivedKey, 0, derivedKey.Length);

            return decryptedVault;
        }

        public string DecryptChecker(byte[] vaultHeaders, char[] password)
        {
            byte[] salt = vaultHeaders.Take(VaultHeaderInfo.SaltSize).ToArray();
            byte[] iv = vaultHeaders.Skip(VaultHeaderInfo.SaltSize).Take(VaultHeaderInfo.IVSize).ToArray();
            byte[] checker = vaultHeaders.Skip(VaultHeaderInfo.SaltSize + VaultHeaderInfo.IVSize)
                                         .Take(VaultHeaderInfo.CheckerSize).ToArray();

            byte[] key = DeriveKeyFromPassword(password, salt, VaultHeaderInfo.IterationNumber);
            byte[] decyptedBytes = Decrypt(checker, key, iv);

            string result = Encoding.UTF8.GetString(decyptedBytes);

            Array.Clear(salt, 0, salt.Length);
            Array.Clear(iv, 0, iv.Length);
            Array.Clear(checker, 0, checker.Length);
            Array.Clear(key, 0, key.Length);
            Array.Clear(decyptedBytes, 0, decyptedBytes.Length);

            return result;

        }

        public byte[] EncryptSecret(string keyValue, char[] secret)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(VaultHeaderInfo.SaltSize);
            byte[] iv = RandomNumberGenerator.GetBytes(VaultHeaderInfo.IVSize);
            byte[] key = DeriveKeyFromPassword(keyValue.ToCharArray(), salt, VaultHeaderInfo.IterationNumber);

            byte[] encryptedData = Encrypt(Encoding.UTF8.GetBytes(secret), key, iv);

            byte[] result = new byte[VaultHeaderInfo.SecretHeaderSize + encryptedData.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(iv, 0, result, salt.Length, iv.Length);
            Buffer.BlockCopy(encryptedData, 0, result, VaultHeaderInfo.SecretHeaderSize, encryptedData.Length);


            Array.Clear(salt, 0, salt.Length);
            Array.Clear(iv, 0, iv.Length);
            Array.Clear(key, 0, key.Length);
            Array.Clear(encryptedData, 0, encryptedData.Length);

            return result;
        }

        public char[] DecryptSecret(byte[] secretBytes, string keyString)
        {
            byte[] salt = secretBytes.Take(VaultHeaderInfo.SaltSize).ToArray();
            byte[] iv = secretBytes.Skip(VaultHeaderInfo.SaltSize).Take(VaultHeaderInfo.IVSize).ToArray();
            byte[] data = secretBytes.Skip(VaultHeaderInfo.SecretHeaderSize).ToArray();

            char[] passwordKey = keyString.ToCharArray();

            byte[] key = DeriveKeyFromPassword(passwordKey, salt, VaultHeaderInfo.IterationNumber);

            byte[] decyptedBytes = Decrypt(data, key, iv);
            char[] result = Encoding.UTF8.GetChars(decyptedBytes);

            Array.Clear(salt, 0, salt.Length);
            Array.Clear(iv, 0, iv.Length);
            Array.Clear(data, 0, data.Length);
            Array.Clear(key, 0, key.Length);
            Array.Clear(passwordKey, 0, passwordKey.Length);
            Array.Clear(decyptedBytes, 0, decyptedBytes.Length);

            return result;

        }


        // Helper Method
        private byte[] DeriveKeyFromPassword(char[] password, byte[] salt, int iterationNumber)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, salt, iterationNumber, HashAlgorithmName.SHA256))
            {
                Array.Clear(passwordBytes, 0, passwordBytes.Length);
                return pbkdf2.GetBytes(VaultHeaderInfo.KeySize);
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
