using PokeCore.Assets;

namespace PokeTools.Assets.Annotations;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AssetSerializerAttribute : AssetPipelineStageAttribute
{
    public AssetSerializerAttribute(AssetType assetType, string displayName) : base(assetType, displayName)
    {
    }
}