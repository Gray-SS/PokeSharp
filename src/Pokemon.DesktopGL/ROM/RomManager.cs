using System;
using System.IO;
using Pokemon.DesktopGL.ROM.Events;
using System.Diagnostics;

namespace Pokemon.DesktopGL.ROM;

public sealed class RomManager
{
    public PokemonRom Rom { get; private set; }
    public bool IsRomLoaded => Rom != null;

    public event EventHandler<RomLoadedArgs> RomLoaded;
    public event EventHandler<RomLoadFailedArgs> RomLoadFailed;

    public bool LoadRom(string romPath)
    {
        if (string.IsNullOrEmpty(romPath) || !File.Exists(romPath))
        {
            RomLoadFailed?.Invoke(this, new RomLoadFailedArgs(romPath, "Invalid ROM path or non-existent file."));
            return false;
        }

        try
        {
            byte[] romData = File.ReadAllBytes(romPath);
            IRomLoader loader = RomLoaderFactory.CreateLoader(romData);

            RomInfo romInfo = loader.LoadRom(romData);
            Debug.Assert(romInfo != null, $"{nameof(romInfo)} was null");

            if (!romInfo.IsPokemonROM)
                throw new NotSupportedException($"The provided ROM isn't a pokémon game: {romInfo}");

            PokemonRomProvider romProvider = PokemonRomProviderFactory.GetProvider(romInfo, romData);
            Debug.Assert(romProvider != null, $"{nameof(romProvider)} was null");

            Rom = new PokemonRom(romInfo, romData, romProvider);
            RomLoaded?.Invoke(this, new RomLoadedArgs(Rom));
            return true;
        }
        catch (NotSupportedException ex)
        {
            RomLoadFailed?.Invoke(this, new RomLoadFailedArgs(romPath, ex.Message));
        }
        catch (Exception ex)
        {
            RomLoadFailed?.Invoke(this, new RomLoadFailedArgs(romPath, $"Unexpected exception occured while loading the ROM. {ex.GetType().Name}: {ex.Message}"));
        }

        return false;
    }
}