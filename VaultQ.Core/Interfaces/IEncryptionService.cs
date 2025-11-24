using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.Core.Interfaces
{
    internal interface IEncryptionService
    {
        byte[] DeriveKeyFromPassword(char[] password);
        string DecryptChecker(byte[] vaultBytes, byte[] derivedKey);
        byte[] EncryptVault(byte[] vaultBytes, byte[] derivedKey);
        byte[] DecryptVault(byte[] vaultBytes, byte[] derivedKey);

        byte[] EncryptSecret(string keyValue, char[] secret);
        char[] DecryptSecret(byte[] secretBytes, string keyString);

    }
}
