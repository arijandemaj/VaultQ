using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultQ.CLI.Commands;

namespace VaultQ.CLI
{
    [Command(Name = "vaultq", Description = "VaultQ CLI Tool")]
    [Subcommand(typeof(SetupCommand), typeof(GetCommand), typeof(AddCommand), typeof(UpdateCommand), typeof(KeysCommand), typeof(RemoveCommand))]
    class Program
    {
        private readonly CommandLineApplication _app;

        public Program(CommandLineApplication app)
        {
            _app = app;
        }

        [Option("-v|--vault <Vault>", Description = "From Vault", ShowInHelpText = true)]
        public string? Vault { get; set; }

        private async Task OnExecute(IConsole console)
        {
            _app.ShowHelp();
        }

        public static void Main(string[] args)
        {
            CommandLineApplication.Execute<Program>(args);
        }

    }
}
