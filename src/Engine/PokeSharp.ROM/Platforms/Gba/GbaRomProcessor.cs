using PokeSharp.Assets;
using PokeSharp.Assets.Exceptions;
using PokeSharp.Assets.VFS;
using PokeSharp.Core.Logging;
using PokeSharp.ROM.Services;

namespace PokeSharp.ROM.Platforms.Gba;

public sealed class GbaRomProcessor : AssetProcessor<RomInfo, Rom>
{
    private readonly ILogger _logger;
    private readonly IGbaConfigProvider _configProvider;
    private readonly IVirtualFileSystem _vfs;

    public GbaRomProcessor(IGbaConfigProvider configProvider, ILogger logger, IVirtualFileSystem vfs)
    {
        _vfs = vfs;
        _logger = logger;
        _configProvider = configProvider;
    }

    public override Rom Process(RomInfo romInfo)
    {
        if (!_configProvider.TryGetConfig(romInfo.GameCode, out GbaConfig? config))
        {
            string details = GetUnsupportedRomDetails();
            throw new AssetProcessorException(
                $"{romInfo} ROM is currently not supported.\n{details}"
            );
        }

        if (!config.SupportsMinimal)
        {
            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(config.Name)) missing.Add("Name");
            if (string.IsNullOrWhiteSpace(config.Language)) missing.Add("Language");
            if (string.IsNullOrWhiteSpace(config.GameCode)) missing.Add("GameCode");
            if (config.Version <= 0) missing.Add("Version");

            throw new AssetProcessorException(
                $"{romInfo} ROM is missing required minimal support fields: {string.Join(", ", missing)}."
            );
        }

        var missingFeatures = config.GetMissingFeatures();
        if (missingFeatures.Any())
        {
            _logger.Warn($"{romInfo} ROM is partially supported. Missing features: {string.Join(", ", missingFeatures)}");
        }

        string scheme = config.VolumeScheme;
        if (_vfs.IsVolumeMounted(scheme))
        {
            _logger.Warn($"Volume at scheme '{scheme}' is already mounted. Unmounting and remounting");
            _vfs.UnmountVolume(scheme);
        }

        var volume = new VolumeInfo(scheme, config.Name, "ROM", FileSystemAccess.Read);
        var rom = new Rom(romInfo, config);

        var vfsBuilder = new GbaVfsBuilder(rom);
        _vfs.MountVolume(volume, new RomFileSystemProvider(volume.RootPath, vfsBuilder));

        return rom;
    }

    private string GetUnsupportedRomDetails()
    {
        var knownRoms = _configProvider.Configs;
        string details;

        if (knownRoms.Count == 0)
        {
            details = "No GBA ROM is currently supported.";
        }
        else
        {
            var romDescriptions = knownRoms.Select(x =>
            {
                if (x.IsFullySupported())
                    return $"- {x}: Fully supported\n";

                var missing = x.GetMissingFeatures();
                if (x.SupportsMinimal)
                    return $"- {x}: Partially supported (missing: {string.Join(", ", missing)})\n";

                return $"- {x}: Not supported (missing minimal requirements)\n";
            });

            details = $"Some GBA ROMs are supported, but not all features may be available.\n{string.Join(string.Empty, romDescriptions)}";
        }

        return details;
    }

    private static string FormatRomSupportDetails(GbaConfig config)
    {
        var missing = config.GetMissingFeatures();

        if (config.IsFullySupported())
            return $"- {config.Name} ({config.GameCode}): Fully supported";

        if (config.SupportsMinimal)
            return $"- {config.Name} ({config.GameCode}): Partially supported (missing: {string.Join(", ", missing)})";

        return $"- {config.Name} ({config.GameCode}): Not supported (missing minimal requirements)";
    }
}