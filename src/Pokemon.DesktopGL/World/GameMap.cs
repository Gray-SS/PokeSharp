using DotTiled;
using DotTiled.Serialization;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core;

namespace Pokemon.DesktopGL.World;

public sealed class GameMap
{
    public string Path { get; }
    public Map TiledMap { get; }

    private static readonly Loader _loader = Loader.Default();

    public GameMap(string path, Map map)
    {
        TiledMap = map;
        Path = path;
    }

    public (int Col, int Row) GetCoord(Vector2 position)
    {
        var col = (int)(position.X / GameConstants.TileSize);
        var row = (int)(position.Y / GameConstants.TileSize);

        return (col, row);
    }

    public bool CollideAt(int col, int row)
    {
        int index = (int)TiledMap.Width * row + col;
        if (col < 0 || row < 0 || col >= TiledMap.Width || row >= TiledMap.Height)
            return true;

        foreach (var layer in TiledMap.Layers)
        {
            if (!layer.Visible || layer is not TileLayer tileLayer)
                continue;

            if (!layer.TryGetProperty<BoolProperty>("is_collidable", out var property))
                continue;

            var isCollidable = property.Value;
            if (!isCollidable)
                continue;

            var gid = tileLayer.Data.Value.GlobalTileIDs.Value[index];
            if (gid != 0) return true;
        }

        return false;
    }

    public Tileset GetTilesetForGid(uint gid)
    {
        foreach (Tileset tileset in TiledMap.Tilesets)
        {
            if (gid >= tileset.FirstGID)
                return tileset;
        }

        return null;
    }

    public static GameMap Load(string mapPath)
    {
        var map = _loader.LoadMap(mapPath);
        return new GameMap(mapPath, map);
    }
}