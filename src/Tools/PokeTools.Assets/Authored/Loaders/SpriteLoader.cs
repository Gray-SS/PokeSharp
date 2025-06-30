using PokeCore.Assets;
using PokeTools.Assets.Authored.Annotations;
using PokeTools.Assets.Authored.Descriptors;

namespace PokeTools.Assets.Authored.Loaders;

[AssetLoader(AssetType.Sprite, "Sprite Loader", Extension = ".sprite")]
public sealed class SpriteLoader : AssetLoader<Sprite, SpriteDescriptor>
{
    public override Sprite Load(Guid assetId, SpriteDescriptor descriptor)
    {
        return new Sprite(
            assetId,
            descriptor.TextureId,
            descriptor.SourceRect
        );
    }
}