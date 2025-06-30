using PokeCore.Assets;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets.Authored.Annotations;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AssetLoaderAttribute : AssetPipelineStageAttribute
{
    public required string Extension { get; init; } = null!;

    public AssetLoaderAttribute(AssetType assetType, string displayName) : base(assetType, displayName)
    {
    }
}