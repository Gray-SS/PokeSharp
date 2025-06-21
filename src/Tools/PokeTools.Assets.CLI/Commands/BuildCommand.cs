using System.ComponentModel;
using PokeTools.Assets.CLI.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PokeTools.Assets.CLI.Commands;

[Description("Build the asset pipeline.")]
public sealed class BuildCommand(
    ICliConsole console
) : Command<BuildCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<source>")]
        [Description("The source directory containing the assets")]
        public string SourceDirectory { get; set; } = null!;
    }

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        if (!Directory.Exists(settings.SourceDirectory))
        {
            string fullPath = Path.GetFullPath(settings.SourceDirectory);
            return ValidationResult.Error($"The source directory '{fullPath}' doesn't exists.");
        }

        return ValidationResult.Success();
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        console.WriteInfo($"Début du build à la source [bold cyan]'{settings.SourceDirectory}'[/]...");

        var status = AnsiConsole.Status();
        status.Spinner = Spinner.Known.Arc;

        status.Start("En cours de configuration", (ctx) =>
        {
            Thread.Sleep(2000);
            ctx.Status = "En cours de préparation";

            Thread.Sleep(2000);
            ctx.Status = "Construction des assets en cours";

            Thread.Sleep(3000);
        });

        console.WriteSuccess("Assets généré avec succès");
        return 0;
    }
}