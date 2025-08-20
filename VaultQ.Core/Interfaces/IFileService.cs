using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultQ.Core.Models;

namespace VaultQ.Core.Interfaces
{
    public interface IFileService
    {
        byte[] SerializeVault(Vault vault);
        Vault LoadDefaultVault();
        bool SaveToFile(byte[] serializedFile, string fileName);
    }
}
