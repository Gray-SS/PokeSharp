using System.ComponentModel;
using PokeCore.IO;
using PokeCore.Assets;
using PokeCore.Common;
using PokeTools.Assets.CLI.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PokeTools.Assets.CLI.Commands;

public sealed class NewCommand(
    ICliConsole console,
    IAssetPipeline assetPipeline
) : AsyncCommand<NewCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-t|--asset-type")]
        [Description("The type of asset to be created")]
        public AssetType AssetType { get; set; }

        [CommandOption("-o|--output")]
        [Description("The path of the created asset")]
        public string OutputPath { get; set; } = null!;
    }

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        if (settings.AssetType == AssetType.None)
            return ValidationResult.Error("The asset type cannot be none, please use '--help' to get further information");

        if (string.IsNullOrWhiteSpace(settings.OutputPath))
            return ValidationResult.Error("No output path specified, please use '--help' to get further information");

        return ValidationResult.Success();
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        VirtualPath outputPath = VirtualPathHelper.ResolvePhysicalPath(settings.OutputPath);
        Result result = await assetPipeline.NewAsync(settings.AssetType, outputPath);
        if (result.IsFailure)
        {
            console.WriteError(result.GetError().Message);
            return 1;
        }

        console.WriteSuccess($"Asset of type '{settings.AssetType}' successfully created and saved to '{settings.OutputPath}'");
        return 0;
    }
}