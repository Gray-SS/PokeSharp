using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Pokemon.DesktopGL.Moves;
using System;
using PokeSharp.Engine.Assets;
using PokeSharp.Engine.Graphics;

namespace Pokemon.DesktopGL.Creatures;

public sealed class CreatureRegistry
{
    private readonly AssetsManager _assetsManager;
    private readonly Dictionary<string, CreatureData> _creatures;

    public CreatureRegistry(AssetsManager assetsManager)
    {
        ArgumentNullException.ThrowIfNull(assetsManager, nameof(assetsManager));

        _assetsManager = assetsManager;
        _creatures = new Dictionary<string, CreatureData>();
    }

    public void Load()
    {
        LoadCreatures("Content/Data/Creatures/creatures.json");
    }

    public CreatureData GetData(string id)
        => _creatures[id];

    private void LoadCreatures(string path)
    {
        string text = File.ReadAllText(path);
        CreatureData[] creatures = JsonSerializer.Deserialize<CreatureData[]>(text);

        foreach (CreatureData data in creatures)
        {
            data.BackSprite = _assetsManager.Load<Sprite>(AssetSource.Content, data.BackSpritePath);
            data.FrontSprite = _assetsManager.Load<Sprite>(AssetSource.Content, data.FrontSpritePath);

            MoveRegistry moveRegistry = PokemonGame.Instance.MoveRegistry;
            data.LearnableMoves = new Dictionary<int, MoveData[]>();
            foreach (var item in data.LearnableMoveIds)
            {
                var moves = new List<MoveData>();
                foreach (string moveId in item.Value)
                {
                    MoveData moveData = moveRegistry.GetData(moveId);
                    moves.Add(moveData);
                }

                data.LearnableMoves.Add(item.Key, [.. moves]);
            }

            _creatures[data.Id] = data;
        }
    }
}