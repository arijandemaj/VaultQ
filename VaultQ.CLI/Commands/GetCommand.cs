using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultQ.CLI.Helpers;

namespace VaultQ.CLI.Commands
{
    [Command(Name = "get", Description = "Get a value by key")]
    internal class GetCommand
    {
        [Option("-k|--key <KEY>", Description = "Key to get", ShowInHelpText = true)]
        public string Key { get; set; }

        private void OnExecute(IConsole console)
        {
            if (string.IsNullOrEmpty(Key))
            {
                console.WriteLine("Error: Key is required.");
                return;
            }

            try
            {
                var password = PasswordHelper.PromptPassword();

                var s = "";
            }
            catch (Exception)
            {
                return;
            }


            console.WriteLine($"Getting key: {Key}");
            // Add your logic here
        }
    }
}
