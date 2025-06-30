using PokeCore.Assets;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets.External.Annotations;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AssetProcessorAttribute : AssetPipelineStageAttribute
{
    public AssetProcessorAttribute(AssetType assetType, string displayName) : base(assetType, displayName)
    {
    }
}