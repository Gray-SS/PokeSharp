using PokeCore.Assets;

namespace PokeTools.Assets.Core;

public readonly struct AssetReference<TAsset>
    where TAsset : IAsset
{
    public bool HasValue => AssetId != null;
    public Guid? AssetId { get; }

    public AssetReference(Guid? assetId)
    {
        AssetId = assetId;
    }
}