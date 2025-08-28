using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.Core.Enums
{
    internal class VaultHeaderInfo
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
                return 16; // 128-bit IV for AES 
            }
        }

        public static int CheckerSize
        {
            get
            {
                return 16; // 128-bit Checker for encryption => overkill
            }
        }

        public static int HeaderSize
        {
            get
            {
                return SaltSize + IVSize + CheckerSize;
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

        public static string VaultChecker
        {
            get
            {
                return "VaultQCheck";
            }
        }

 

    }
}
