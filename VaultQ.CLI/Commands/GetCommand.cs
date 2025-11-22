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
    [Command(Name = "get", Description = "Get a value by key")]
    internal class GetCommand
    {
        private readonly VaultManager vaultManager;
        public GetCommand(Program parent)
        {
            vaultManager = VaultManager.CreateDefault(parent.Vault);
        }

        [Option("-k|--key <KEY>", Description = "Key to get", ShowInHelpText = true)]
        public string Key { get; set; }

        private async Task OnExecute(IConsole console)
        {
            var password = InputHelper.Input("password: ", true);

            bool verifyPassword = await vaultManager.AuthenticateVault(password);

            if (!verifyPassword)
            {
                Console.WriteLine("Incorrect Password, Try Again!");
                return;
            }

            char[] retrivedSecret = vaultManager.GetSecret(Key);

            Console.Write("\r\n");
            for (int i = 0; i < retrivedSecret.Length; i++)
            {
                Console.Write(retrivedSecret[i]);
            }
            Console.Write("\r\n");
            Array.Clear(retrivedSecret, 0, retrivedSecret.Length);

        }
    }
}
