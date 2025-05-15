using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace PokeSharp.Core.Graphics;

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

    public SpriteSheet(Sprite baseSprite, int? cols, int? rows, int? spriteWidth, int? spriteHeight)
    {
        BaseSprite = baseSprite;
        if (rows.HasValue && cols.HasValue)
        {
            Rows = rows.Value;
            Columns = cols.Value;
            SpriteWidth = baseSprite.Width / Columns;
            SpriteHeight = baseSprite.Height / Rows;
        }
        else if (spriteWidth.HasValue && spriteHeight.HasValue)
        {
            SpriteWidth = spriteWidth.Value;
            SpriteHeight = spriteHeight.Value;
            Columns = baseSprite.Width / SpriteHeight;
            Rows = baseSprite.Height / SpriteHeight;
        }
        else throw new InvalidOperationException("Couldn't instantiate a sprite sheet without the pair of colums and rows or the pair of sprite width and sprite height");

        TotalCount = Columns * Rows;

        _sprites = BakeSprites();
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