using Microsoft.Xna.Framework.Graphics;
using PokeCore.Common;
using PokeCore.IO;
using PokeCore.Logging;
using PokeTools.Assets.Raw;

namespace PokeTools.Assets.Importers;

public sealed class SpriteImporter : AssetImporter<RawSprite>
{
    public override Type ProcessorType => throw new NotImplementedException();

    public override string SupportedExtensions => throw new NotImplementedException();

    private readonly Logger _logger;
    private readonly GraphicsDevice _graphicsDevice;

    public SpriteImporter(Logger<SpriteImporter> logger, GraphicsDevice graphicsDevice)
    {
        _logger = logger;
        _graphicsDevice = graphicsDevice;
    }

    public override Result<RawSprite, string> Import(IVirtualFile file)
    {
        using Stream stream = file.OpenRead();
        if (!stream.CanRead)
            return Result.Failed("File stream is not readable.");

        _logger.Trace($"Creating texture from file stream '{file.Path}'");
        Texture2D texture = Texture2D.FromStream(_graphicsDevice, stream);
        ResetGraphicsDeviceState(_graphicsDevice);
        _logger.Trace($"Texture created: {texture.Width}x{texture.Height}");

        var sprite = new RawSprite(texture, null);
        return Result.Success(sprite);
    }

    public static void ResetGraphicsDeviceState(GraphicsDevice device)
    {
        device.Textures[0] = null;
        device.SamplerStates[0] = SamplerState.LinearClamp;
        device.BlendState = BlendState.AlphaBlend;
        device.DepthStencilState = DepthStencilState.None;
        device.RasterizerState = RasterizerState.CullNone;
    }
}