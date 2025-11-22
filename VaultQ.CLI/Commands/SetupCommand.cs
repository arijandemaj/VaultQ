using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultQ.CLI.Helpers;
using VaultQ.Core;
using VaultQ.Core.Services;

namespace VaultQ.CLI.Commands
{
    [Command(Name =  "setup", Description = "Setup the vault")]
    internal class SetupCommand
    {
        private readonly VaultManager vaultManager;

        public SetupCommand()
        {
            vaultManager = VaultManager.CreateDefault(null);
        }

        private async Task OnExecute(IConsole console)
        {
            Console.Write("Vault Name (leave blank for default): ");
            string? vaultName = Console.ReadLine();

            if (string.IsNullOrEmpty(vaultName))
                vaultName = "Default";

            char[]? password = null;
            char[]? confirmPassword = null;
             
            while (true) 
            {
                if (password == null)
                {

                    char[]? passwordInput = InputHelper.Input("Vault Password: ", true);

                    if (passwordInput.Length == 0)
                    {
                        Console.WriteLine("Password can not be empty, try again!");
                        continue;
                    }

                    password = new char[passwordInput.Length];
                    Array.Copy(passwordInput, password, passwordInput.Length);
                    Array.Clear(passwordInput, 0, passwordInput.Length);

                }

                if (confirmPassword == null)
                {

                    char[]? confirmPasswordInput = InputHelper.Input("Confirm Vault Password: ", true);

                    if (confirmPasswordInput.Length == 0)
                    {
                        Console.WriteLine("Confirm password can not be empty, try again!");
                        continue;
                    }


                    confirmPassword = new char[confirmPasswordInput.Length];
                    Array.Copy(confirmPasswordInput, confirmPassword, confirmPasswordInput.Length);
                    Array.Clear(confirmPasswordInput, 0, confirmPasswordInput.Length);
                }

                if (!password.SequenceEqual(confirmPassword))
                {
                    Console.WriteLine("Password Don't match try again");

                    password = null;
                    confirmPassword = null;

                    continue;
                }

                Array.Clear(confirmPassword, 0, confirmPassword.Length);
                break;

            }

            try
            {
                await vaultManager.SetupVault(vaultName, password);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Something went wrong!");
            }
            finally
            {
                Array.Clear(password, 0, password.Length);
            }

            Console.WriteLine("\r\nVault Created Successfuly!\r\n");
           

        }

    }
}
