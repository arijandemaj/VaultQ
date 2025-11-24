using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.Core.Models
{
    [MessagePackObject(AllowPrivate = true)]
    internal class Vault
    {
        public Vault()
        {
            Name = "";
            Data = new Dictionary<string, byte[]>();
        }

        [Key(0)]
        public required string Name { get; set; }
        [Key(1)]
        public required Dictionary<string, byte[]> Data { get; set; }

    }
}
