using PokeCore.Assets;

namespace PokeTools.Assets.Annotations;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AssetImporterAttribute : AssetPipelineStageAttribute
{
    public bool IsAuthored { get; init; } = false;
    public required string[] SupportedExtensions { get; init; }

    public AssetImporterAttribute(AssetType assetType, string displayName) : base(assetType, displayName)
    {
    }
}