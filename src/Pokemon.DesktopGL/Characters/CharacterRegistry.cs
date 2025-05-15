using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PokeSharp.Core.Graphics;
using PokeSharp.Core.Managers;

namespace Pokemon.DesktopGL.Characters;

public sealed class CharacterRegistry
{
    private readonly AssetsManager _assetsManager;
    private readonly Dictionary<string, CharacterData> _characters;

    public CharacterRegistry(AssetsManager assetsManager)
    {
        _assetsManager = assetsManager;
        _characters = new Dictionary<string, CharacterData>();
    }

    public void Load()
    {
        LoadCharactersData("Content/Data/Characters/npcs.json");
        LoadCharactersData("Content/Data/Characters/trainers.json");
        LoadCharactersData("Content/Data/Characters/players.json");
    }

    public CharacterData Get(string id)
        => _characters[id];

    private void LoadCharactersData(string path)
    {
        string text = File.ReadAllText(path);
        CharacterData[] datas = JsonSerializer.Deserialize<CharacterData[]>(text);

        foreach (CharacterData data in datas)
        {
            Sprite sprite = _assetsManager.LoadSprite(data.SpritesheetPath);
            SpriteSheet sheet = new SpriteSheet(sprite, 4, 4, null, null);

            data.IdleAnimations = new AnimationPack(new Dictionary<string, Animation>()
            {
                { "Down", Animation.FromSpriteSheet(sheet, 0) },
                { "Left", Animation.FromSpriteSheet(sheet, 4) },
                { "Right", Animation.FromSpriteSheet(sheet, 8) },
                { "Up", Animation.FromSpriteSheet(sheet, 12) },
            });

            data.RunAnimations = new AnimationPack(new Dictionary<string, Animation>()
            {
                { "Down", Animation.FromSpriteSheet(sheet, 0, 3) },
                { "Left", Animation.FromSpriteSheet(sheet, 4, 7) },
                { "Right", Animation.FromSpriteSheet(sheet, 8, 11) },
                { "Up", Animation.FromSpriteSheet(sheet, 12, 15) },
            });

            _characters.Add(data.Id, data);
        }
    }
}