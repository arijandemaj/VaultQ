using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.Core.Models
{
    internal class VaultSession
    {
        public byte[] DerivedKey { get; }
        public Vault Vault { get; }
        public bool IsAuthenticated
        {
            get
            {
                return Vault is not null;
            }
        }

        public VaultSession(Vault vault, byte[] derivedKey)
        {
            Vault = vault;
            DerivedKey = derivedKey;
        }

        public void Clear()
        {
            Array.Clear(DerivedKey);
         
        }

    }
}
