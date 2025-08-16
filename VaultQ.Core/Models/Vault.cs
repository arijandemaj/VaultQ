using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.Core.Models
{
    internal class Vault
    {
        public required string Name { get; set; }
        
        public Dictionary<string, string>? Data { get; set; }

    }
}
