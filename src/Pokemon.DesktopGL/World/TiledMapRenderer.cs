using DotTiled;
using Pokemon.DesktopGL.Core.Renderers;

namespace Pokemon.DesktopGL.World;

public sealed class TiledMapRenderer
{
    public Map Map { get; }

    public TiledMapRenderer(Map map)
    {
        Map = map;
    }

    public void Draw(GameRenderer renderer)
    {
        foreach (BaseLayer layer in Map.Layers)
        {
            if (!layer.Visible || layer is not TileLayer tileLayer)
                continue;

            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    int index = y * (int)Map.Width + x;
                    if (!tileLayer.Data.HasValue)
                        continue;

                    // var data = tileLayer.Data.Value;
                    // var gid = data.GlobalTileIDs.Value.;

                    // if (gid == 0) continue;

                    // var tileset = GetTilesetForGid(gid);
                    // if (tileset == null) continue;

                    // int localId = (int)(gid - tileset.FirstGid);

                    // int tilesPerRow = tileset.Image.Width / tileset.TileWidth;
                    // int sx = (localId % tilesPerRow) * tileset.TileWidth;
                    // int sy = (localId / tilesPerRow) * tileset.TileHeight;

                    // var srcRect = new Rectangle(sx, sy, tileset.TileWidth, tileset.TileHeight);
                    // var dstPos = new Vector2(x * _map.TileWidth, y * _map.TileHeight);

                    // Texture2D texture = _tilesetTextures[tileset.Image.Source];

                    // spriteBatch.Draw(texture, dstPos, srcRect, Color.White);
                }
            }
        }
    }

    private Tileset GetTilesetForGid(int gid)
    {
        foreach (Tileset tileset in Map.Tilesets)
        {
            if (gid >= tileset.FirstGID)
                return tileset;
        }

        return null;
    }
}