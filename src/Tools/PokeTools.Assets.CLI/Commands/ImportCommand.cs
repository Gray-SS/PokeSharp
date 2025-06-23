using System.ComponentModel;
using System.Runtime.InteropServices;
using PokeCore.Common;
using PokeCore.IO;
using PokeCore.IO.Services;
using PokeTools.Assets.CLI.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PokeTools.Assets.CLI.Commands;

[Description("Import the specified resource")]
public sealed class ImportCommand : AsyncCommand<ImportCommand.Settings>
{
    private readonly ICliConsole _console;
    private readonly IAssetPipeline _assetPipeline;
    private readonly IVirtualFileSystem _vfs;

    public ImportCommand(ICliConsole console, IAssetPipeline assetPipeline, IVirtualFileSystem vfs)
    {
        _vfs = vfs;
        _console = console;
        _assetPipeline = assetPipeline;
    }

    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<path>")]
        [Description("The resource path to be imported")]
        public string ResourcePath { get; set; } = null!;
    }

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        var path = VirtualPathHelper.ResolvePhysicalPath(settings.ResourcePath);
        if (!_vfs.FileExists(path))
            return ValidationResult.Error($"The resource path '{Path.GetFullPath(settings.ResourcePath)}' doesn't exists.");

        return ValidationResult.Success();
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        _console.WriteInfo($"Importing asset at path: '{settings.ResourcePath}'");

        var path = VirtualPathHelper.ResolvePhysicalPath(settings.ResourcePath);
        IAssetImporter[] importers = _assetPipeline.FindImportersForExtension(path.Extension).ToArray();

        IAssetImporter targetImporter;
        if (importers.Length == 0)
        {
            _console.WriteWarn($"No importer found to import '{path.Extension}' resources");
            // Maybe ask to choose an importer from the registered importers
            return 1;
        }
        else if (importers.Length > 1)
        {
            _console.WriteInfo($"Multiple importers found to import '{path.Extension}' resources");

            var importersType = importers.Select(x => x.GetType());
            var importersName = importersType.Select(x => x.Name).ToArray();

            string selectedImporter = _console.SelectionPrompt(
                "Please select one importer:",
                "[grey](Move up and down to reveal more importers)[/]",
                importersName
            );

            targetImporter = importers.First(x => x.GetType().Name == selectedImporter);
        }
        else targetImporter = importers.First();

        Result result = await _assetPipeline.ImportAsync(targetImporter, path);
        if (result.IsFailure)
        {
            Error error = result.GetError();
            _console.WriteError(error.Message);
            return 1;
        }

        _console.WriteSuccess($"Asset imported successfully.");
        return 0;
    }
}