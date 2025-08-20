using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.Core.Interfaces
{
    public interface IEncryptionService
    {
        byte[] EncryptVault(byte[] vaultBytes, string password);
        byte[] DecryptVault(byte[] vaultBytes, string password);

    }
}
