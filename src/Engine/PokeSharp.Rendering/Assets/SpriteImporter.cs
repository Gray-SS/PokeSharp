using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Assets;
using PokeSharp.Assets.Exceptions;
using PokeSharp.Assets.VFS;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Threadings;
using PokeSharp.Rendering.Assets.Raw;

namespace PokeSharp.Rendering.Assets;

public sealed class SpriteImporter : AssetImporter<RawSprite>
{
    public override Type ProcessorType => typeof(SpriteProcessor);
    public override string SupportedExtensions => ".png,.jpeg,.jpg";

    private readonly Logger _logger;
    private readonly GraphicsDevice _graphicsDevice;

    public SpriteImporter(Logger logger, GraphicsDevice graphicsDevice)
    {
        _logger = logger;
        _graphicsDevice = graphicsDevice;
    }

    public override RawSprite Import(IVirtualFile file)
    {
        using Stream stream = file.OpenRead();
        if (!stream.CanRead)
            throw new AssetImporterException("File stream is not readable.");

        _logger.Trace($"Creating texture from file stream '{file.Path}'");
        Texture2D texture = Texture2D.FromStream(_graphicsDevice, stream);
        ResetGraphicsDeviceState(_graphicsDevice);
        _logger.Trace($"Texture created: {texture.Width}x{texture.Height}");

        return new RawSprite(texture, null);
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