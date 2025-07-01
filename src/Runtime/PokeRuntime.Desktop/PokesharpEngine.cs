using System;
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
    private RuntimeSprite _sprite;

    private readonly IAssetManager _assetManager;

    public PokesharpEngine(EngineConfiguration config, IAssetManager assetManager) : base(config)
    {
        _assetManager = assetManager;
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        Resolution.SetResolution(ResolutionSize.R1280x720);

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _assetManager.LoadBundle("Content/mygame.bundle");
        _sprite = (RuntimeSprite)_assetManager.Load(Guid.Parse("89b3a43c-d2f5-4d46-ac21-142614c9450a"));
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

        if (_sprite != null && _sprite.Texture != null)
        {
            Rectangle? rect = _sprite.TextureRegion switch
            {
                System.Drawing.Rectangle region => new Rectangle(region.X, region.Y, region.Width, region.Height),
                null => null
            };

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_sprite.Texture.GraphicsTexture, new Rectangle(100, 100, 320 / 2, 480 / 2), rect, Color.White);
            _spriteBatch.End();
        }

        base.OnDraw(gameTime);
    }
}