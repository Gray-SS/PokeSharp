using System;
using System.Linq;
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
        const float epsilon = 0.001f;

        int col = (int)MathF.Floor((position.X + epsilon) / GameConstants.TileSize);
        int row = (int)MathF.Floor((position.Y + epsilon) / GameConstants.TileSize);

        return (col, row);
    }

    public int GetData(string layerName, int col, int row)
    {
        var layer = TiledMap.Layers.FirstOrDefault(x => x.Name == layerName);
        if (layer == null || layer is not TileLayer tileLayer) return -1;

        int index = row * (int)TiledMap.Width + col;
        uint gid = tileLayer.Data.Value.GlobalTileIDs.Value[index];

        var tileset = GetTilesetForGid(gid);
        if (tileset == null) return -1;

        return (int)(gid - tileset.FirstGID.Value);
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