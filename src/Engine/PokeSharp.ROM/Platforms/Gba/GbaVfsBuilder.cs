using System.Text;
using PokeSharp.Core.Logging;
using PokeSharp.ROM.Services;
using PokeSharp.ROM.Utils;

namespace PokeSharp.ROM.Platforms.Gba;

public sealed class GbaVfsBuilder : RomVfsBuilder
{
    public RomReader<GbaPointer> Reader { get; }

    private readonly Logger _logger;

    public GbaVfsBuilder(Rom rom) : base(rom)
    {
        _logger = LoggerFactory.GetLogger(typeof(GbaVfsBuilder));

        Reader = new RomReader<GbaPointer>(rom.RawData);
    }

    public override void Build(RomDirectory root)
    {
        _logger.Info($"Extraction started for '{Rom.Config.Name}'");
        _logger.Debug($"Target directory: '{root.Path}'");

        if (Rom.Config.SupportsSpeciesLoading)
        {
            RomDirectory graphicsRoot = root.AddDirectory("Pokémons");
            BuildSpecies(graphicsRoot);
        }
    }

    private void BuildSpecies(RomDirectory root)
    {
        _logger.Info($"Extracting pokémons");

        GbaPokemonsConfig config = Rom.Config.Pokemons;
        GbaPointer namesPointer = GbaPointer.FromRaw(config.Names);

        for (int i = 0; i < config.Count; i++)
        {
            int address = namesPointer.PhysicalAddress + i * config.NameLength;

            ReadOnlySpan<byte> text = Reader.ReadSpan(address, config.NameLength);

            string name = GbaTextDecoder.Decode(text);
            root.AddFile($"{name}.txt", Encoding.UTF8.GetBytes(name));
        }

        _logger.Info($"Extracted pokémons at '{root.Path}'");
    }
}