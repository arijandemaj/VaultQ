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

        public void SetupVault(string vaultName, string vaultPassword)
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

                fileService.SaveToFile(encryptedVault, vaultName + ".dat");
            }
            catch (Exception ex)
            {
                throw new Exception("Something Went Wrong!");
            }
        }
    }
}
