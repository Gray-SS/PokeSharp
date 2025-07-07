using PokeCore.Assets;
using PokeCore.Common.Serializations;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.Pipeline.Importers;

namespace PokeTools.Assets.Types.Sprite;

[AssetImporter(AssetType.Sprite, "Sprite Importer", SupportedExtensions = [".sprite"], IsAuthored = true)]
public sealed class SpriteImporter : AuthoredAssetImporter<RawSprite>
{
    public SpriteImporter(IYamlSerializer yamlSerializer) : base(yamlSerializer)
    {
    }
}
