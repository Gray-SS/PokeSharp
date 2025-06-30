using PokeCore.Assets;

namespace PokeTools.Assets.Annotations;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AssetCompilerAttribute : AssetPipelineStageAttribute
{
    public AssetCompilerAttribute(AssetType assetType, string displayName) : base(assetType, displayName)
    {
    }
}