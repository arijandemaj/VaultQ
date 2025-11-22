using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.Core.Exceptions
{
    public class VaultQNotInitializedException : Exception
    {
        private const string message = "VaultQ is not initialized. Run `vaultq setup` first.";

        public VaultQNotInitializedException() : base(message) 
        { }

        public VaultQNotInitializedException(Exception exception) : base(message, exception)
        { }
    }
}
