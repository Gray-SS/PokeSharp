using System.ComponentModel;
using Spectre.Console.Cli;

namespace PokeTools.Assets.CLI.Commands;

public sealed class ImportCommand : Command<ImportCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<path>")]
        [Description("The input path to be build")]
        public string InputPath { get; set; } = null!;

        [CommandOption("-o|--output")]
        [Description("The path of the builded asset")]
        public string OutputPath { get; set; } = null!;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        

        return 0;
    }
}