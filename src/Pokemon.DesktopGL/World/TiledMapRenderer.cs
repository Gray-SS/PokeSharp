using System.Collections.Generic;
using System.IO;
using DotTiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.Core;
using Pokemon.DesktopGL.Core.Renderers;

using Color = Microsoft.Xna.Framework.Color;

namespace Pokemon.DesktopGL.World;

public sealed class TiledMapRenderer
{
    public GameMap Map { get; }
    private Map TiledMap => Map.TiledMap;

    private readonly Dictionary<string, Texture2D> _textures;

    public TiledMapRenderer(GameMap map)
    {
        Map = map;

        _textures = LoadTilesetsTexture();
    }

    private Dictionary<string, Texture2D> LoadTilesetsTexture()
    {
        var content = PokemonGame.Instance.Content;
        var textures = new Dictionary<string, Texture2D>();

        var mapSource = Path.GetFullPath(Map.Path);
        var mapSourceDir = Path.GetDirectoryName(mapSource);

        foreach (var tileset in TiledMap.Tilesets)
        {
            var tilesetSource = Path.Combine(mapSourceDir, tileset.Source.Value);
            var tilesetSourceDir = Path.GetDirectoryName(tilesetSource);

            var tilesetImageSource = tileset.Image.Value.Source.Value;
            var imageSource = Path.Combine(tilesetSourceDir, tilesetImageSource);

            var relativePath = Path.GetRelativePath(content.RootDirectory, imageSource);

            var relativePathWithoutExtension = Path.Combine(
                Path.GetDirectoryName(relativePath) ?? string.Empty,
                Path.GetFileNameWithoutExtension(relativePath)
            ).Replace('\\', '/');

            textures[tilesetImageSource] = content.Load<Texture2D>(relativePathWithoutExtension);
        }

        return textures;
    }

    public void Draw(GameRenderer renderer)
    {
        foreach (BaseLayer layer in TiledMap.Layers)
        {
            if (!layer.Visible || layer is not TileLayer tileLayer)
                continue;

            for (int y = 0; y < TiledMap.Height; y++)
            {
                for (int x = 0; x < TiledMap.Width; x++)
                {
                    int index = y * (int)TiledMap.Width + x;
                    if (!tileLayer.Data.HasValue)
                        continue;

                    var data = tileLayer.Data.Value;
                    uint gid = data.GlobalTileIDs.Value[index];

                    if (gid == 0) continue;

                    var tileset = GetTilesetForGid(gid);
                    if (tileset == null) continue;

                    int localId = (int)(gid - tileset.FirstGID);

                    int tilesPerRow = (int)(tileset.Image.Value.Width.Value / tileset.TileWidth);
                    int sx = localId % tilesPerRow * (int)tileset.TileWidth;
                    int sy = localId / tilesPerRow * (int)tileset.TileHeight;

                    var srcRect = new Rectangle(sx, sy, (int)tileset.TileWidth, (int)tileset.TileHeight);
                    var dstRect = new Rectangle(x * GameConstants.TileSize, y * GameConstants.TileSize, GameConstants.TileSize, GameConstants.TileSize);

                    Texture2D texture = _textures[tileset.Image.Value.Source.Value];
                    renderer.Draw(texture, dstRect, srcRect, Color.White);
                }
            }
        }
    }

    private Tileset GetTilesetForGid(uint gid)
    {
        foreach (Tileset tileset in TiledMap.Tilesets)
        {
            if (gid >= tileset.FirstGID)
                return tileset;
        }

        return null;
    }
}