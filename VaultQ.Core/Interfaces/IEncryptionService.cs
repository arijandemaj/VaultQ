using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.Core.Interfaces
{
    internal interface IEncryptionService
    {
        byte[] EncryptVault(byte[] vaultBytes, char[] password);
        byte[] DecryptVault(byte[] vaultBytes, char[] password);
        string DecryptChecker(byte[] vaultBytes, char[] password);
        byte[] EncryptSecret(string keyValue, char[] secret);
        char[] DecryptSecret(byte[] secretBytes, string keyString);

    }
}
