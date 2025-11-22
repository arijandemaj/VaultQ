using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultQ.CLI.Helpers;
using VaultQ.Core;

namespace VaultQ.CLI.Commands
{
    [Command(Name = "keys", Description = "get list of keys")]
    internal class KeysCommand
    {
        private readonly VaultManager vaultManager;

        public KeysCommand(Program parent)
        {
            vaultManager = VaultManager.CreateDefault(parent.Vault);
        }

        private async Task OnExecute(IConsole console)
        {
            var password = InputHelper.Input("password: ", true);

            bool verifyPassword = await vaultManager.AuthenticateVault(password);

            if (!verifyPassword)
            {
                Console.WriteLine("Incorrect Password, Try Again!");
                return;
            }

            var keys = vaultManager.GetKeys();

            Console.Write("\r\n");
            for (int i = 0; i < keys.Length; i++)
            {
                Console.WriteLine($"{i + 1}) {keys[i]}");
            }

        }
    }
}
