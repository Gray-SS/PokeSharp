using System.ComponentModel;
using PokeCore.Common;
using PokeCore.IO;
using PokeCore.IO.Services;
using PokeTools.Assets.CLI.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PokeTools.Assets.CLI.Commands;


[Description("Build the asset pipeline.")]
public sealed class BuildManifestCommand(
    ICliConsole console,
    IVirtualFileSystem vfs,
    IAssetBuildServices buildServices
) : AsyncCommand<BuildManifestCommand.Settings>
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

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        var path = VirtualPathHelper.ResolvePhysicalPath(settings.InputPath);
        if (path.IsFile)
            return ValidationResult.Error("The input path must be a directory path.");

        if (!vfs.DirectoryExists(path))
            return ValidationResult.Error("No directory exists at provided path.");

        return ValidationResult.Success();
    }

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var inputPath = VirtualPathHelper.ResolvePhysicalPath(settings.InputPath);
        Result result = buildServices.BuildManifest(inputPath);
        if (result.TryGetError(out Error? error))
        {
            console.WriteError(error.Message);
            return Task.FromResult(1);
        }

        console.WriteSuccess("Manifest successfully built.");
        return Task.FromResult(0);
    }
}