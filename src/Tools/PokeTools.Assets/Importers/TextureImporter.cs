using PokeCore.Common;
using PokeTools.Assets.Processors;
using PokeTools.Assets.Objects.Raw;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PokeTools.Assets.Importers;

public sealed class TextureImporter : AssetImporter<RawTexture>
{
    public override Type ProcessorType => typeof(TextureProcessor);
    public override string[] SupportedExtensions => [".png", ".jpeg", ".jpg"];

    public override Result<RawTexture> Import(Stream stream)
    {
        return Result.Catch(() =>
        {
            var image = Image.Load<Rgba32>(stream);

            var data = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(data);

            return new RawTexture(image.Width, image.Height, data);
        });
    }
}