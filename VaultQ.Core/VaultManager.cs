using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultQ.Core.Enums;
using VaultQ.Core.Exceptions;
using VaultQ.Core.Interfaces;
using VaultQ.Core.Models;
using VaultQ.Core.Services;

namespace VaultQ.Core
{
    public class VaultManager
    {
        private readonly IEncryptionService encryptionService;
        private readonly IFileService fileService;
   
        private Vault? Vault;
        private char[] VaultPassword;
        private string? VaultName;

        private VaultManager(IEncryptionService encryptionService, IFileService fileService, string? vaultName)
        {
            this.encryptionService = encryptionService;
            this.fileService = fileService;
            this.VaultName = vaultName;
        }

        public static VaultManager CreateDefault(string? vaultName)
        {
            return new VaultManager(new EncryptionService(), new FileService(), vaultName);
        }

        public async Task SetupVault(string vaultName, char[] vaultPassword)
        {
            try
            {
                var vaultCreation = new Vault
                {
                    Name = vaultName,
                    Data = new Dictionary<string, byte[]>()
                };

                byte[] serializedVault = fileService.SerializeVault(vaultCreation);
                byte[] encryptedVault = encryptionService.EncryptVault(serializedVault, vaultPassword);

                await fileService.SaveSetup(encryptedVault, vaultName + ".dat");
            }
            catch (Exception ex)
            {
                // TODO: Add custom exceptions
                throw new Exception("Something Went Wrong!");
            }
        }

        public async Task<bool> AuthenticateVault(char[] vaultPassword)
        {
            if (string.IsNullOrEmpty(this.VaultName))
            {
                VaultName = await fileService.GetDefaultVaultName() ?? throw new VaultQNotInitializedException();
            }

            if (!VaultName.EndsWith(".dat"))
            {
                VaultName += ".dat";
            }

            try
            {
                var vaultHeaders = await fileService.ReadVaultHeaders(this.VaultName);
                var decryptChecker = encryptionService.DecryptChecker(vaultHeaders, vaultPassword);

                bool isValid = decryptChecker == VaultHeaderInfo.VaultChecker;

                if (!isValid)
                    return false;

                var vaultBytes = await fileService.GetVaultBytes(this.VaultName);
                var decryptBytes = encryptionService.DecryptVault(vaultBytes, vaultPassword);
                this.Vault = fileService.DeserializeVault(decryptBytes);
                this.VaultPassword = vaultPassword;

                return isValid;
            }
            catch
            {
                return false;
            }
        }

        public async Task AddSecret(string key, char[] secret)
        {
            try
            {
                var encryptedSecret = encryptionService.EncryptSecret(key, secret);

                if (Vault == null || Vault.Data == null || string.IsNullOrEmpty(VaultName))
                    return;

                Vault.Data.Add(key, encryptedSecret);

                var serializedVault = fileService.SerializeVault(Vault);
                byte[] encryptedVault = encryptionService.EncryptVault(serializedVault, this.VaultPassword);
                await fileService.SaveVault(encryptedVault, VaultName);
            }
            catch (Exception ex)
            {
                // TODO: Add custom exceptions
                throw new Exception("Something Went Wrong!");
            }

        }

        public async Task UpdateSecret(string key, char[] secret)
        {
            try
            {
                var encryptedSecret = encryptionService.EncryptSecret(key, secret);

                if (Vault == null || Vault.Data == null || string.IsNullOrEmpty(VaultName))
                    return;

                Vault.Data[key] = encryptedSecret;

                var serializedVault = fileService.SerializeVault(Vault);
                byte[] encryptedVault = encryptionService.EncryptVault(serializedVault, this.VaultPassword);
                await fileService.SaveVault(encryptedVault, VaultName);
            }
            catch (Exception ex)
            {
                // TODO: Add custom exceptions
                throw new Exception("Something Went Wrong!");
            }

        }

        public char[] GetSecret(string key)
        {
            try
            {
                if (Vault == null || Vault.Data == null || string.IsNullOrEmpty(VaultName))
                    return Array.Empty<char>();

                byte[] secret = Vault.Data[key];
                char[] decryptedSecret = encryptionService.DecryptSecret(secret, key);

                return decryptedSecret;
            }
            catch (Exception ex)
            {
                // TODO: Add custom exceptions
                throw new Exception("Something Went Wrong!");
            }

        }

        public async Task DeleteSecret(string key)
        {
            try
            {
                if (Vault == null || Vault.Data == null || string.IsNullOrEmpty(VaultName))
                    return;


                Vault.Data.Remove(key);

                var serializedVault = fileService.SerializeVault(Vault);
                byte[] encryptedVault = encryptionService.EncryptVault(serializedVault, this.VaultPassword);
                await fileService.SaveVault(encryptedVault, VaultName);

            }
            catch (Exception ex)
            {
                // TODO: Add custom exceptions
                throw new Exception("Something Went Wrong!");
            }

        }

        public string[] GetKeys()
        {
            try
            {
                if (Vault == null || Vault.Data == null || string.IsNullOrEmpty(VaultName))
                    return Array.Empty<string>();

                var data = Vault.Data.Keys.ToArray();
                return data;
            }
            catch (Exception ex)
            {
                // TODO: Add custom exceptions
                throw new Exception("Something Went Wrong!");
            }
        }

    }
}
