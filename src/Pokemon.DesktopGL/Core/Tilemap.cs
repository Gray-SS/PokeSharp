using System.Collections.Generic;
using System.Linq;

namespace Pokemon.DesktopGL.Core;

public sealed class Tilemap
{
    public int MapWidth { get; }
    public int MapHeight { get; }

    public int[] Data { get; }
    public Sprite[] Tiles { get; }

    public Tilemap(int mapWidth, int mapHeight, IEnumerable<Sprite> tiles, int[] data)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;

        Data = data;
        Tiles = [.. tiles];
    }

    public int GetData(int index)
    {
        if (index < 0 || index >= Data.Length)
            return -1;

        return Data[index];
    }

    public int GetData(int col, int row)
        => GetData(row * MapWidth + col);

    public Sprite GetTile(int index)
        => Tiles[Data[index]];

    public Sprite GetTile(int col, int row)
        => GetTile(row * MapWidth + col);
}