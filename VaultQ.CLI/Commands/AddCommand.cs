using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VaultQ.CLI.Helpers;
using VaultQ.Core;

namespace VaultQ.CLI.Commands
{
    [Command(Name = "add", Description = "Add a key value pair")]
    internal class AddCommand
    {
        private readonly VaultManager vaultManager;
        private readonly Program parent;

        public AddCommand(Program parent)
        {
            this.parent = parent;
            vaultManager = VaultManager.CreateDefault();
        }

        [Option("-k|--key <KEY>", Description = "Key to get", ShowInHelpText = true)]
        public required string Key { get; set; }

        private async Task OnExecute(IConsole console)
        {
            var password = InputHelper.Input("password: ", true);

            bool verifyPassword = await vaultManager.AuthenticateAsync(password, parent.Vault);

            if (!verifyPassword)
            {
                Console.WriteLine("Incorrect Password, Try Again!");
                return;
            }

            var secret = InputHelper.Input("Secret: ", false);

            if (secret != null)
                await vaultManager.WriteSecretAsync(Key, secret, false);
        }
    }
}
