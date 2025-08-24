using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultQ.Core.Interfaces;
using VaultQ.Core.Models;
using VaultQ.Core.Services;

namespace VaultQ.Core
{
    public class VaultManager
    {
        private readonly IEncryptionService encryptionService;
        private readonly IFileService fileService;

        private VaultManager(IEncryptionService encryptionService, IFileService fileService)
        {
            this.encryptionService = encryptionService;
            this.fileService = fileService;
        }

        public static VaultManager CreateDefault()
        {
            return new VaultManager(new EncryptionService(), new FileService());
        }

        public async Task SetupVault(string vaultName, char[] vaultPassword)
        {
            try
            {
                var vaultCreation = new Vault
                {
                    Name = vaultName,
                    Data = new Dictionary<string, string>()
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

        public async Task<bool> IsSetupDone()
        {
            try
            {
                bool setupDone = await fileService.GetDefaultVaultName() != null;
                return setupDone;
            }
            catch(Exception ex)
            {
                // TODO: Add custom exceptions
                throw new Exception("Something Went Wrong!");
            }
        }

        private async Task<Vault> GetVault(string vaultName, char[] vaultPassword)
        {
            var vaultBytes = await fileService.GetVaultBytes(vaultName);
            var decryptBytes = encryptionService.DecryptVault(vaultBytes, vaultPassword);
            Vault vault = fileService.DeserializeVault(decryptBytes);
            return vault;
        }

    }
}
