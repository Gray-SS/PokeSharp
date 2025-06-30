using PokeCore.Assets;

namespace PokeRuntime.Assets;

public sealed class RuntimeTexture : Texture, IRuntimeAsset
{
    public MGGraphics.Texture2D GraphicsTexture { get; }

    public RuntimeTexture(MGGraphics.GraphicsDevice graphics, Guid id, int width, int height, byte[] data) : base(id, width, height, data)
    {
        GraphicsTexture = new MGGraphics.Texture2D(graphics, width, height);
        GraphicsTexture.SetData(data);
    }
}