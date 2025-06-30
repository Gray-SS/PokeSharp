using PokeCore.Assets;

namespace PokeTools.Assets.Annotations;

[AttributeUsage(AttributeTargets.Class)]
public abstract class AssetPipelineStageAttribute : Attribute
{
    public string DisplayName { get; }
    public AssetType AssetType { get; }

    public AssetPipelineStageAttribute(AssetType assetType, string displayName)
    {
        AssetType = assetType;
        DisplayName = displayName;
    }
}