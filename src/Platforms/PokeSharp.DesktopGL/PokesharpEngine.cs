using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ninject;
using PokeSharp.Core;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules;
using PokeSharp.Core.Resolutions;
using PokeSharp.Inputs;

namespace PokeSharp.DesktopGL;

public class PokesharpEngine : Engine
{
    public PokesharpEngine(IKernel kernel, ILogger logger, IModuleLoader moduleLoader) : base(kernel, logger, moduleLoader)
    {
    }

    protected override void OnInitialize()
    {
        Resolution.SetResolution(ResolutionSize.R1280x720);
        base.OnInitialize();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);

        if (Input.IsKeyPressed(Keys.F11))
        {
            Resolution.ToggleFullScreen();
        }

        if (Input.IsKeyPressed(Keys.Escape))
        {
            Exit();
        }
    }

    protected override void OnDraw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.OnDraw(gameTime);
    }
}