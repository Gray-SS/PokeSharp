using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pokemon.DesktopGL.Core;

public sealed class SpriteSheet
{
    public int SpriteWidth { get; }
    public int SpriteHeight { get; }

    public int Rows { get; }
    public int Columns { get; }

    public int TotalCount { get; }

    public Sprite BaseSprite { get; }
    public IReadOnlyCollection<Sprite> Sprites => _sprites.Values;

    private readonly Dictionary<int, Sprite> _sprites;

    public SpriteSheet(Sprite baseSprite, int cols, int rows)
    {
        BaseSprite = baseSprite;

        Rows = rows;
        Columns = cols;
        SpriteWidth = baseSprite.Width / cols;
        SpriteHeight = baseSprite.Height / rows;
        TotalCount = cols * rows;

        _sprites = BakeSprites();
    }

    public static SpriteSheet FromDimension(Sprite baseSprite, int spriteWidth, int spriteHeight)
    {
        int cols = baseSprite.Width / spriteWidth;
        int rows = baseSprite.Height / spriteHeight;
        return new SpriteSheet(baseSprite, cols, rows);
    }

    public Sprite GetSprite(int data)
        => _sprites[data];

    public Sprite GetSprite(int col, int row)
        => _sprites[row * col + col];

    private Dictionary<int, Sprite> BakeSprites()
    {
        var sprites = new Dictionary<int, Sprite>();

        for (int i = 0; i < TotalCount; i++)
        {
            int col = i % Columns;
            int row = i / Columns;

            var sourceRect = new Rectangle(col * SpriteWidth, row * SpriteHeight, SpriteWidth, SpriteHeight);
            var sprite = new Sprite(BaseSprite.Texture, sourceRect);

            sprites.Add(i, sprite);
        }

        return sprites;
    }
}