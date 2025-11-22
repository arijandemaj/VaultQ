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
    [Command(Name = "remove", Description = "Remove a key value pair")]
    internal class RemoveCommand
    {
        private readonly VaultManager vaultManager;

        public RemoveCommand(Program parent)
        {
            vaultManager = VaultManager.CreateDefault(parent.Vault);
        }

        [Option("-k|--key <KEY>", Description = "Key to get", ShowInHelpText = true)]
        public required string Key { get; set; }

        private async Task OnExecute(IConsole console)
        {
            var password = InputHelper.Input("password: ", true);

            bool verifyPassword = await vaultManager.AuthenticateVault(password);

            if (!verifyPassword)
            {
                Console.WriteLine("Incorrect Password, Try Again!");
                return;
            }

            await vaultManager.DeleteSecret(Key);

        }
    }
}
