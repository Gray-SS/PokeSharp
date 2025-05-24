using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PokeSharp.Core;
using PokeSharp.Core.Resolutions;
using PokeSharp.Inputs;

namespace PokeSharp.Editor;

public sealed class EditorEngine : Engine
{
    public EditorEngine(EngineConfiguration config) : base(config)
    {
        Window.AllowUserResizing = true;
    }

    protected override void OnInitialize()
    {
        Resolution.SetResolution(ResolutionSize.R1920x1080);

        base.OnInitialize();
    }

    protected override void OnLoad()
    {
        base.OnLoad();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);

        if (Input.IsKeyPressed(Keys.Escape))
        {
            Exit();
        }

        if (Input.IsKeyPressed(Keys.F11))
        {
            Resolution.ToggleFullScreen();
        }
    }

    protected override void OnDraw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.OnDraw(gameTime);
    }
}