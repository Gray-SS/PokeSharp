using Microsoft.Xna.Framework;
using Ninject;
using PokeSharp.Core;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules;
using PokeSharp.Core.Resolutions;

namespace PokeSharp.Editor;

public sealed class EditorEngine : Engine
{
    public EditorEngine(IKernel kernel, ILogger logger, IModuleLoader moduleLoader) : base(kernel, logger, moduleLoader)
    {
    }

    protected override void OnInitialize()
    {
        Resolution.SetResolution(ResolutionSize.R800x600);
        base.OnInitialize();
    }

    protected override void OnDraw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.PaleVioletRed);
        base.OnDraw(gameTime);
    }
}