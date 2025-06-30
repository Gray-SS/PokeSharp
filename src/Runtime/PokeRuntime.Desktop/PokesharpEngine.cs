using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PokeEngine.Core;
using PokeEngine.Core.Resolutions;
using PokeEngine.Inputs;
using PokeRuntime.Assets;

namespace PokeRuntime.Desktop;

public class PokesharpEngine : BaseEngine
{
    private SpriteBatch _spriteBatch;
    private RuntimeTexture _textureAsset;

    private readonly IAssetManager _assetManager;

    public PokesharpEngine(EngineConfiguration config, IAssetManager assetManager) : base(config)
    {
        _assetManager = assetManager;
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        Resolution.SetResolution(new ResolutionSize(1208, 151));

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // _textureAsset = _assetManager.Load<RuntimeTexture>("../../Tools/PokeTools.Assets.CLI/assets/milkshake.png.asset");
        RuntimeSprite sprite = _assetManager.Load<RuntimeSprite>(assetId: 0);
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

        _spriteBatch.Begin();
        _spriteBatch.Draw(_textureAsset.GraphicsTexture, new Rectangle(100, 100, 500, 500), Color.White);
        _spriteBatch.End();

        base.OnDraw(gameTime);
    }
}