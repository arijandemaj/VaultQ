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
        private IEncryptionService _encryptionService;
        private IFileService _fileService;
        private VaultSession _vaultSession;
 
        private VaultManager(IEncryptionService encryptionService, IFileService fileService)
        {
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public static VaultManager CreateDefault()
        {
            return new VaultManager(new EncryptionService(), new FileService());
        }

        public async Task<bool> AuthenticateAsync(char[] vaultPassword, string? vaultName)
        {
            if (vaultPassword.Length == 0)
                return false;

            if (string.IsNullOrEmpty(vaultName))
                vaultName = await _fileService.GetDefaultVaultName() ?? throw new VaultQNotInitializedException();

            byte[] derivedKey = _encryptionService.DeriveKeyFromPassword(vaultPassword);

            var vaultHeaders = await _fileService.ReadVaultHeaders(vaultName);
            var decryptChecker = _encryptionService.DecryptChecker(vaultHeaders, derivedKey);

            bool isValid = decryptChecker == VaultHeaderInfo.VaultChecker;

            if (!isValid)
            {
                Array.Clear(derivedKey, 0, derivedKey.Length);
                return false;
            }

            var vaultBytes = await _fileService.GetVaultBytes(vaultName);
            var decryptedVaultBytes = _encryptionService.DecryptVault(vaultBytes, derivedKey);
            var vaultDeserialized = _fileService.DeserializeVault(decryptedVaultBytes);

            _vaultSession = new VaultSession(vaultDeserialized, derivedKey);

            Array.Clear(vaultBytes, 0, vaultBytes.Length);
            Array.Clear(decryptedVaultBytes, 0, decryptedVaultBytes.Length);

            return true;
        }

        public async Task SetupVaultAsync(string vaultName, char[] vaultPassword)
        {
            if (string.IsNullOrEmpty(vaultName) || vaultPassword.Length == 0)
                return;

            if (!vaultName.EndsWith(".dat"))
                vaultName += ".dat";

            var vaultCreation = new Vault
            {
                Name = vaultName,
                Data = new Dictionary<string, byte[]>()
            };

            byte[] derivedKey = _encryptionService.DeriveKeyFromPassword(vaultPassword);

            byte[] serializedVault = _fileService.SerializeVault(vaultCreation);
            byte[] encryptedVault = _encryptionService.EncryptVault(serializedVault, derivedKey);

            await _fileService.SaveSetup(encryptedVault, vaultName);

            Array.Clear(derivedKey, 0, derivedKey.Length);
            Array.Clear(serializedVault, 0, serializedVault.Length);
            Array.Clear(encryptedVault, 0, encryptedVault.Length);
        }

        public async Task WriteSecretAsync(string key, char[] secret, bool overwrite)
        {
            if (string.IsNullOrEmpty(key) || secret.Length == 0)
                return;

            if (!_vaultSession.IsAuthenticated)
                return;

            var encryptedSecret = _encryptionService.EncryptSecret(key, secret);

            if (overwrite)
                _vaultSession.Vault.Data[key] = encryptedSecret;
            else
                _vaultSession.Vault.Data.Add(key, encryptedSecret);

            Array.Clear(secret, 0, secret.Length);
            await SaveVaultHelper();
            _vaultSession.Clear();

        }

        public async Task DeleteSecretAsync(string key)
        {
            if (!_vaultSession.IsAuthenticated)
                return;

            _vaultSession.Vault.Data.Remove(key);

            await SaveVaultHelper();
            _vaultSession.Clear();
        }

        public char[] GetSecret(string key)
        {
            if (string.IsNullOrEmpty(key) || !_vaultSession.IsAuthenticated)
                return [];

            if (_vaultSession.Vault.Data.TryGetValue(key, out var encryptedSecret) || encryptedSecret is null)
                return [];

            char[] decryptedSecret = _encryptionService.DecryptSecret(encryptedSecret, key);

            Array.Clear(encryptedSecret, 0, encryptedSecret.Length);
            _vaultSession.Clear();
            return decryptedSecret;
        }

        public string[] GetKeys()
        {
            if (!_vaultSession.IsAuthenticated)
                return [];

            var data = _vaultSession.Vault.Data.Keys.ToArray();
            _vaultSession.Clear();
            return data;
        }
        
        // Helper Methods
        private async Task SaveVaultHelper()
        {
            byte[] serializedVault = _fileService.SerializeVault(_vaultSession.Vault);
            byte[] encryptedVault = _encryptionService.EncryptVault(serializedVault, _vaultSession.DerivedKey);
            await _fileService.SaveVault(encryptedVault, _vaultSession.Vault.Name);

            Array.Clear(serializedVault, 0, serializedVault.Length);
            Array.Clear(encryptedVault, 0, encryptedVault.Length);
        }
    }
}
