using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.Core.Enums
{
    internal class VaultHeaderSizes
    {
        public static int SaltSize 
        {
            get
            {
                return 16; // 128 bits Salt
            }
        }
        public static int IVSize
        {
            get
            {
                return 15; // 128-bit IV for AES 
            }
        }

        public static int HeaderSize
        {
            get
            {
                return SaltSize + IVSize;
            }
        }

        public static int KeySize
        {
            get
            {
                return 32; // 256 bits key 
            }
        }

        public static int IterationNumber
        {
            get
            {
                return 100_000;
            }
        }

 

    }
}
