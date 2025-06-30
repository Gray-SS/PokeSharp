using PokeCore.Assets;
using PokeTools.Assets.Annotations;

namespace PokeTools.Assets.External.Annotations;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AssetImporterAttribute : AssetPipelineStageAttribute
{
    public required string[] SupportedExtensions { get; init; }

    public AssetImporterAttribute(AssetType assetType, string displayName) : base(assetType, displayName)
    {
    }
}