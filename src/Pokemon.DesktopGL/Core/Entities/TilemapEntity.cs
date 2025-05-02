using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core.Renderers;

namespace Pokemon.DesktopGL.Core.Entities;

public sealed class TilemapEntity
{
    public Vector2 Position { get; set; }

    private readonly Tilemap _tilemap;
    private readonly int _tileSize;

    public TilemapEntity(Tilemap tilemap, int tileSize)
    {
        Position = Vector2.Zero;

        _tilemap = tilemap;
        _tileSize = tileSize;
    }

    public (int Col, int Row) GetCoord(Vector2 position)
    {
        var col = (int)((position.X + Position.X) / _tileSize);
        var row = (int)((position.Y + Position.Y) / _tileSize);

        return (col, row);
    }

    public int GetData(int col, int row)
        => _tilemap.GetData(col, row);

    public int GetDataAtPosition(Vector2 position)
    {
        var col = (int)((position.X + Position.X) / _tileSize);
        var row = (int)((position.Y + Position.Y) / _tileSize);

        return _tilemap.GetData(col, row);
    }

    public void Draw(GameRenderer renderer)
    {
        for (int y = 0; y < _tilemap.MapHeight; y++)
        {
            for (int x = 0; x < _tilemap.MapWidth; x++)
            {
                Sprite tile = _tilemap.GetTile(x, y);

                var position = new Vector2(x * _tileSize, y * _tileSize);
                var size = new Vector2(_tileSize);

                renderer.Draw(tile, position, size, Color.White);
            }
        }
    }
}