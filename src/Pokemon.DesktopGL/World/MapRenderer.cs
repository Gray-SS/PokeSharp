using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core;
using Pokemon.DesktopGL.Core.Renderers;

namespace Pokemon.DesktopGL.World;

public sealed class MapRenderer
{
    public Vector2 Position { get; set; }

    private readonly MapData _map;
    private readonly int _tileSize;

    public MapRenderer(MapData map, int tileSize)
    {
        Position = Vector2.Zero;

        _map = map;
        _tileSize = tileSize;
    }

    public (int Col, int Row) GetCoord(Vector2 position)
    {
        var col = (int)((position.X + Position.X) / _tileSize);
        var row = (int)((position.Y + Position.Y) / _tileSize);

        return (col, row);
    }

    public int GetData(int col, int row)
        => _map.GetData(col, row);

    public int GetDataAtPosition(Vector2 position)
    {
        var col = (int)((position.X + Position.X) / _tileSize);
        var row = (int)((position.Y + Position.Y) / _tileSize);

        return _map.GetData(col, row);
    }

    public void Draw(GameRenderer renderer)
    {
        for (int y = 0; y < _map.MapHeight; y++)
        {
            for (int x = 0; x < _map.MapWidth; x++)
            {
                Sprite tile = _map.GetTile(x, y);

                var position = new Vector2(x * _tileSize, y * _tileSize);
                var size = new Vector2(_tileSize);

                renderer.Draw(tile, position, size, Color.White);
            }
        }
    }
}