using System.ComponentModel;
using Spectre.Console.Cli;

namespace PokeTools.Assets.CLI.Commands;

[Description("Import the specified resource")]
public sealed class ImportCommand : AsyncCommand<ImportCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<resource_path>")]
        [Description("The path of the resource")]
        public string ResourcePath { get; set; } = null!;
    }

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        return Task.FromResult(0);
    }
}