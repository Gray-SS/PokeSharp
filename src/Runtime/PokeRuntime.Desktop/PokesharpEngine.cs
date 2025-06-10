using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PokeEngine.Core;
using PokeEngine.Core.Resolutions;
using PokeEngine.Inputs;

namespace PokeRuntime.Desktop;

public class PokesharpEngine : BaseEngine
{
    public PokesharpEngine(EngineConfiguration config) : base(config)
    {
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        Resolution.SetResolution(ResolutionSize.R1280x720);
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