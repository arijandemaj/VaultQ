using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultQ.CLI.Helpers;
using VaultQ.Core.Services;

namespace VaultQ.CLI.Commands
{
    [Command(Name =  "setup", Description = "Setup the vault")]
    internal class SetupCommand
    {
        private void OnExecute(IConsole console)
        {
            Console.Write("Vault Name (leave blank for default): ");
            string? vaultName = Console.ReadLine();

            if (string.IsNullOrEmpty(vaultName))
                vaultName = "Default";

            string? password = null;
            string? confirmPassword = null;
             
            while (true) 
            {
                if (string.IsNullOrEmpty(password))
                {
                    Console.Write("Vault Password: ");
                    string? passwordInput = Console.ReadLine();

                    if (string.IsNullOrEmpty(passwordInput))
                    {
                        Console.WriteLine("Password can not be empty, try again!");
                        continue;
                    }

                    if (!PasswordHelper.ValidatePassword(passwordInput))
                    {
                        Console.WriteLine("Password should be minimum 6 length characters");
                        continue;
                    }

                    password = passwordInput;

                }

                if (string.IsNullOrEmpty(confirmPassword))
                {
                    Console.Write("Confirm Vault Password: ");
                    string? confirmPasswordInput = Console.ReadLine();

                    if (string.IsNullOrEmpty(confirmPasswordInput))
                    {
                        Console.WriteLine("Confirm password can not be empty, try again!");
                        continue;
                    }

                    if (!PasswordHelper.ValidatePassword(confirmPasswordInput))
                    {
                        Console.WriteLine("Password should be minimum 6 length characters");
                        continue;
                    }

                    confirmPassword = confirmPasswordInput;
                }

                if (!string.Equals(password, confirmPassword))
                {
                    Console.WriteLine("Password Don't match try again");

                    password = null;
                    confirmPassword = null;

                    continue;
                }

                break;

            }

            var vaultService = new VaultService();
            vaultService.SetupVault(vaultName, password);

        }

    }
}
