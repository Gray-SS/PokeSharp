using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Engine.Managers;
using PokeSharp.ROM;
using PokeSharp.ROM.Descriptors;

namespace PokeSharp.Engine.Assets;

public abstract class RomAssetImporter<T> : AssetImporter<T> where T : class
{
    public PokemonRom Rom => RomManager.Rom!;
    public RomManager RomManager { get; }

    protected RomAssetImporter(RomManager romManager, GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        RomManager = romManager;
    }

    public override bool CanImport(AssetReference assetRef)
    {
        if (assetRef.Source != AssetSource.ROM)
            return false;

        if (!RomManager.IsRomLoaded)
            throw new InvalidOperationException("No ROM has been loaded.");

        if (assetRef.Payload is not IRomDescriptor)
            throw new InvalidOperationException("Expected an IRomDescriptor payload for ROM asset.");

        return true;
    }
}