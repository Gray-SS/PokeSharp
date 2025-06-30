using System.ComponentModel;
using PokeCore.Assets;
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
    IAssetPipeline assetPipeline
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
                return ValidationResult.Error($"No output path provided. Use help for further information");
        }
        else if (!vfs.DirectoryExists(path))
            ValidationResult.Error($"No file or directory exists at path '{Path.GetFullPath(settings.InputPath)}'.");

        return ValidationResult.Success();
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var inputPath = VirtualPathHelper.ResolvePhysicalPath(settings.InputPath);
        if (inputPath.IsFile)
        {
            var outputPath = VirtualPathHelper.ResolvePhysicalPath(settings.OutputPath);

            Result buildResult = await assetPipeline.BuildAsync(inputPath, outputPath);
            if (buildResult.IsFailure)
            {
                console.WriteError(buildResult.GetError().Message);
                return 1;
            }

            console.WriteSuccess("Asset successfully built.");
        }
        else if (inputPath.IsDirectory)
        {
            Result buildResult = assetPipeline.BuildBundle(inputPath, "my_bundle");
            if (buildResult.IsFailure)
            {
                console.WriteError(buildResult.GetError().Message);
                return 1;
            }

            console.WriteSuccess("Asset bundle successfully built.");
        }

        return 0;
    }

    // private void ConfigureImportParameters(ImportParameter[] parameters)
    // {
    //     foreach (ImportParameter parameter in parameters)
    //     {
    //         bool configure = console.ConfirmPrompt($"[dim]Configure[/] [bold cyan]{parameter.DisplayName}[/] [dim italic]({parameter.GetValue()})[/] [dim]?[/]");
    //         if (!configure) continue;

    //         Type ptype = parameter.ParameterType;
    //         if (ptype.IsEnum)
    //         {
    //             string selected = console.SelectionPrompt(string.Empty, "[grey](Move up and down to reveal more options)[/]", Enum.GetNames(ptype));
    //             parameter.SetValue(Enum.Parse(ptype, selected));
    //         }
    //         else if (ptype == typeof(byte)) parameter.SetValue(console.TextPrompt<byte>("Enter your value (byte):"));
    //         else if (ptype == typeof(short)) parameter.SetValue(console.TextPrompt<short>("Enter your value (short):"));
    //         else if (ptype == typeof(int)) parameter.SetValue(console.TextPrompt<int>("Enter your value (int):"));
    //         else if (ptype == typeof(long)) parameter.SetValue(console.TextPrompt<long>("Enter your value (long):"));
    //         else if (ptype == typeof(sbyte)) parameter.SetValue(console.TextPrompt<sbyte>("Enter your value (sbyte):"));
    //         else if (ptype == typeof(ushort)) parameter.SetValue(console.TextPrompt<ushort>("Enter your value (ushort):"));
    //         else if (ptype == typeof(uint)) parameter.SetValue(console.TextPrompt<uint>("Enter your value (uint):"));
    //         else if (ptype == typeof(ulong)) parameter.SetValue(console.TextPrompt<ulong>("Enter your value (ulong):"));
    //         else if (ptype == typeof(char)) parameter.SetValue(console.TextPrompt<char>("Enter your value (char):"));
    //         else if (ptype == typeof(float)) parameter.SetValue(console.TextPrompt<float>("Enter your value (float):"));
    //         else if (ptype == typeof(double)) parameter.SetValue(console.TextPrompt<double>("Enter your value (double):"));
    //         else if (ptype == typeof(string)) parameter.SetValue(console.TextPrompt<string>("Enter your value (string):"));
    //         else console.WriteWarn($"Parameters of type '{ptype.Name}' are currently not supported.");
    //     }
    // }
}