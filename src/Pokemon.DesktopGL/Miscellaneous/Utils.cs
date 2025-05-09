using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Core;

namespace Pokemon.DesktopGL.Miscellaneous;

public static class Utils
{
    public static (int Col, int Row) ConvertMapPosToTileCoord(Vector2 mapPosition)
    {
        var map = PokemonGame.Instance.ActiveWorld.Map.TiledMap;
        int col = (int)(mapPosition.X / map.TileWidth);
        int row = (int)(mapPosition.Y / map.TileHeight);

        return (col, row);
    }

    public static Vector2 ConvertTileCoordToWorldPos((int Col, int Row) coord)
        => new Vector2(coord.Col, coord.Row) * GameConstants.TileSize;

    public static Vector2 ConvertMapPosToWorldPos(Vector2 mapPosition)
        => ConvertTileCoordToWorldPos(ConvertMapPosToTileCoord(mapPosition));

}