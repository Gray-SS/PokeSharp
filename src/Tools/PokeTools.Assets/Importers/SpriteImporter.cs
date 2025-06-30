using PokeCore.Assets;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.Intermediate;

namespace PokeTools.Assets.Importers;

[AssetImporter(AssetType.Sprite, "Sprite Importer", SupportedExtensions = [ ".sprite" ], IsAuthored = true)]
public sealed class SpriteImporter : AuthoredAssetImporter<RawSprite>;