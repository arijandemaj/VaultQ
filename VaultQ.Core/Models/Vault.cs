using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.Core.Models
{
    [MessagePackObject]
    public class Vault
    {
        [Key(0)]
        public required string Name { get; set; }
        [Key(1)]
        public Dictionary<string, string>? Data { get; set; }

    }
}
