using PokeCore.Assets;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using PokeTools.Assets.Annotations;
using PokeTools.Assets.Pipeline.Importers;
using PokeCore.Common.Results;

namespace PokeTools.Assets.Types.Texture;

[AssetImporter(AssetType.Texture, "Texture Importer", SupportedExtensions = [".png", ".jpeg", ".jpg"])]
public sealed class TextureImporter : AssetImporter<RawTexture>
{
    public override Result<RawTexture> Import(Stream stream)
    {
        var image = Image.Load<Rgba32>(stream);

        var data = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(data);

        return new RawTexture(image.Width, image.Height, data);
    }
}