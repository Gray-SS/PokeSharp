using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PokeSharp.Assets;
using PokeSharp.Core;
using PokeSharp.Core.Resolutions;
using PokeSharp.Entities;
using PokeSharp.Inputs;
using PokeSharp.ROM;
using PokeSharp.Scenes;

namespace PokeSharp.DesktopGL;

public class PokesharpEngine : Engine
{
    private Scene _scene;
    private AssetPipeline _assetPipeline;

    public PokesharpEngine(EngineConfiguration config) : base(config)
    {
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        Resolution.SetResolution(ResolutionSize.R1280x720);

        _assetPipeline = ServiceLocator.GetService<AssetPipeline>();
        var rom = _assetPipeline.Load<Rom>("/home/sklin/Documents/dev/PokeSharp/roms/firered.gba");


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

        _scene?.Update(gameTime);
    }

    protected override void OnDraw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _scene?.Draw(gameTime);

        base.OnDraw(gameTime);
    }
}