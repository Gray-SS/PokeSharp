using PokeCore.Assets;

namespace PokeTools.Assets.Annotations;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AssetProcessorAttribute : AssetPipelineStageAttribute
{
    public AssetProcessorAttribute(AssetType assetType, string displayName) : base(assetType, displayName)
    {
    }
}