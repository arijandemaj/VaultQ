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
    [Command(Name = "update", Description = "Update a key value pair")]
    internal class UpdateCommand
    {
        private readonly VaultManager vaultManager;

        public UpdateCommand(Program parent)
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

            var secret = InputHelper.Input("Secret: ", false);

            if (secret != null)
                await vaultManager.UpdateSecret(Key, secret);
        }
    }
}
