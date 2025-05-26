using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Core;
using PokeSharp.Core.Resolutions;

namespace PokeSharp.Entities;

public class Entity
{
    public string Id { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public Color Color { get; set; }

    private float _timer;
    private Vector2 _basePosition;
    private Vector2 _position;

    private Texture2D _texture = null!;
    private SpriteBatch _spriteBatch = null!;

    public virtual void OnLoad()
    {
        var graphicsDevice = Engine.Instance.GraphicsDevice;

        var res = Resolution.ResolutionSize;
        _basePosition = new Vector2(Random.Shared.NextSingle() * res.Width, Random.Shared.NextSingle() * res.Height);

        _spriteBatch = new SpriteBatch(graphicsDevice);
        _texture = new Texture2D(graphicsDevice, 1, 1);
        _texture.SetData([Color.White]);
    }

    public virtual void OnUpdate(GameTime gameTime)
    {
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        _position = _basePosition + new Vector2(MathF.Sin(_timer * 2f) * 100f + 100f, 0.0f);
    }

    public virtual void OnDraw(GameTime gameTime)
    {
        _spriteBatch.Begin();
        Rectangle rect = new Rectangle((int)_position.X, (int)_position.Y, 100, 100);
        _spriteBatch.Draw(_texture,  rect, Color);

        _spriteBatch.End();
    }
}