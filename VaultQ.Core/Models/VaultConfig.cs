using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.Core.Models
{
    internal class VaultConfig
    {
        public string VaultName { get; set; }
        public bool DefaultVault { get; set; }
    }

    internal class VaultConfigFile
    {
        public VaultConfigFile()
        {
            Vaults = new List<VaultConfig>();
        }

        public List<VaultConfig> Vaults { get; set; }
    }

}
