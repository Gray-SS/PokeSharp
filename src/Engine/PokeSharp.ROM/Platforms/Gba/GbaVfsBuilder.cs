using System.Text;
using PokeSharp.Core.Logging;
using PokeSharp.ROM.Services;
using PokeSharp.ROM.Utils;

namespace PokeSharp.ROM.Platforms.Gba;

public sealed class GbaVfsBuilder : RomVfsBuilder
{
    public RomReader<GbaPointer> Reader { get; }

    private readonly ILogger _logger;

    public GbaVfsBuilder(Rom rom) : base(rom)
    {
        _logger = LoggerFactory.GetLogger(typeof(GbaVfsBuilder));

        Reader = new RomReader<GbaPointer>(rom.RawData);
    }

    public override void Build(RomDirectory root)
    {
        _logger.Info($"Building and extraction started for '{Rom.Config.Name}'");
        _logger.Debug($"Target extraction directory: '{root.Path}'");

        if (Rom.Config.SupportsSpeciesLoading)
        {
            RomDirectory graphicsRoot = root.AddDirectory("Pokémons");
            BuildSpecies(graphicsRoot);
        }
    }

    private void BuildSpecies(RomDirectory root)
    {
        _logger.Info($"Building species at '{root.Path}'");

        GbaPokemonsConfig config = Rom.Config.Pokemons;
        GbaPointer namesPointer = GbaPointer.FromRaw(config.Names);

        for (int i = 0; i < config.Count; i++)
        {
            _logger.Info($"[{i + 1}/{config.Count}] Extracting pokémon name");
            int address = namesPointer.PhysicalAddress + i * config.NameLength;

            ReadOnlySpan<byte> text = Reader.ReadSpan(address, config.NameLength);
            string name = GbaTextDecoder.Decode(text);
            _logger.Info($"[{i + 1}/{config.Count}] Pokémon name decoded as '{name}'");

            RomFile pokemonNameFile = root.AddFile($"{name}.txt", Encoding.UTF8.GetBytes(name));
            _logger.Info($"[{i + 1}/{config.Count}] Storing name at '{pokemonNameFile.Path}'");
        }
    }
}