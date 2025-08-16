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
    [Subcommand(typeof(SetupCommand), typeof(GetCommand))]
    class Program
    {
        public static void Main(string[] args) 
        {
            CommandLineApplication.Execute<Program>(args);
        }

    }
}
