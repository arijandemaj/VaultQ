using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultQ.Core.Models;

namespace VaultQ.Core.Interfaces
{
    internal interface IFileService
    {
        byte[] SerializeVault(Vault vault);
        Vault LoadDefaultVault();
        void SaveSetup(byte[] serializedFile, string fileName);
        bool DefaultVaultExists();
    }
}
