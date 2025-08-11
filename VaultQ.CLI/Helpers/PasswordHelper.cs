using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultQ.CLI.Helpers
{
    internal static class PasswordHelper
    {
        public static void PromptPassword()
        {
            Console.Write("passwod: ");

            string? typedPassword = Console.ReadLine();

            if (string.IsNullOrEmpty(typedPassword)) 
            {
                throw new Exception("Wrong Password");
            }
        }

    }
}
