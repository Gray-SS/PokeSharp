using PokeCore.Assets;
using PokeCore.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using PokeTools.Assets.External.Intermediate;
using PokeTools.Assets.External.Annotations;

namespace PokeTools.Assets.External.Importers;

[AssetImporter(AssetType.Texture, "Texture Importer", SupportedExtensions = [".png", ".jpeg", ".jpg"])]
public sealed class TextureImporter : AssetImporter<RawTexture>
{
    public override Result<RawTexture> Import(Stream stream)
    {
        var image = Image.Load<Rgba32>(stream);

        var data = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(data);

        return Result<RawTexture>.Success(new RawTexture(image.Width, image.Height, data));
    }
}