using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Pokemon.DesktopGL.Core.Managers;

namespace Pokemon.DesktopGL.Creatures;

public sealed class CreatureRegistry
{
    private readonly AssetsManager _assetsManager;
    private readonly Dictionary<string, CreatureData> _creatures;

    public CreatureRegistry(AssetsManager assetsManager)
    {
        _assetsManager = assetsManager;
        _creatures = new Dictionary<string, CreatureData>();
    }

    public void Load()
    {
        LoadFromPath("Content/Data/Pokemons/pokemons.json");
    }

    public CreatureData Get(string id)
        => _creatures[id];


    private void LoadFromPath(string path)
    {
        string text = File.ReadAllText(path);
        CreatureData[] creatures = JsonSerializer.Deserialize<CreatureData[]>(text);

        foreach (CreatureData data in creatures)
        {
            data.BackSprite = _assetsManager.LoadSprite(data.BackSpritePath);
            data.FrontSprite = _assetsManager.LoadSprite(data.FrontSpritePath);

            _creatures[data.Id] = data;
        }
    }
}