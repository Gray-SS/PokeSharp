using System.ComponentModel;
using PokeCore.Common;
using PokeCore.IO;
using PokeCore.IO.Services;
using PokeTools.Assets.CLI.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PokeTools.Assets.CLI.Commands;

[Description("Build the asset pipeline.")]
public sealed class BuildCommand(
    ICliConsole console,
    IVirtualFileSystem vfs,
    IAssetBuildServices buildServices
) : AsyncCommand<BuildCommand.Settings>
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
        if (vfs.FileExists(path))
        {
            if (string.IsNullOrWhiteSpace(settings.OutputPath))
            {
                settings.OutputPath = Path.ChangeExtension(settings.InputPath, ".asset");
                console.WriteInfo($"No output path specified, using '{settings.OutputPath}'");
            }
        }
        else if (vfs.DirectoryExists(path))
            return ValidationResult.Error($"Directory aren't supported at the moment.");
        else
            return ValidationResult.Error($"File or directory not found at path '{settings.InputPath}'");

        return ValidationResult.Success();
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var inputPath = VirtualPathHelper.ResolvePhysicalPath(settings.InputPath);
        if (inputPath.IsFile)
        {
            var outputPath = VirtualPathHelper.ResolvePhysicalPath(settings.OutputPath);

            Result buildResult = await buildServices.BuildAsync(inputPath, outputPath);
            if (buildResult.IsFailure)
            {
                console.WriteError(buildResult.GetError().Message);
                return 1;
            }

            console.WriteSuccess("Asset successfully built.");
        }

        return 0;
    }
}