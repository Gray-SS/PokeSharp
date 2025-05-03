using DotTiled.Serialization;

namespace Pokemon.DesktopGL.World;

public sealed class GameMap
{
    public string Path { get; }
    public DotTiled.Map TiledMap { get; }

    private static readonly Loader _loader = Loader.Default();

    public GameMap(string path, DotTiled.Map map)
    {
        TiledMap = map;
        Path = path;
    }

    public static GameMap Load(string mapPath)
    {
        var map = _loader.LoadMap(mapPath);
        return new GameMap(mapPath, map);
    }
}