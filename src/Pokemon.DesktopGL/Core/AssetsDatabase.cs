// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Text.Json;
// using Microsoft.Xna.Framework.Content;
// using Microsoft.Xna.Framework.Graphics;
// using Pokemon.DesktopGL.Characters;
// using Pokemon.DesktopGL.Core.Graphics;

// namespace Pokemon.DesktopGL.Core;

// public class AssetsDatabase
// {
//     private readonly ContentManager _content;

//     private readonly Dictionary<string, Sprite> _sprites;
//     private readonly Dictionary<string, SpriteSheet> _spriteSheets;
//     private readonly Dictionary<string, CharacterData> _characters;

//     public AssetsDatabase(ContentManager content)
//     {
//         _content = content;
//         _sprites = new Dictionary<string, Sprite>();
//         _spriteSheets = new Dictionary<string, SpriteSheet>();
//         _characters = new Dictionary<string, CharacterData>();
//     }

//     public void Load()
//     {
//         LoadSpritesheets("Data/spritesheets.json");
//     }

//     private void LoadSpritesheets(string path)
//     {
//         string text = File.ReadAllText(path);
//         SpriteSheetData[] datas = JsonSerializer.Deserialize<SpriteSheetData[]>(text);

//         foreach (SpriteSheetData data in datas)
//         {
//             Sprite sprite = LoadSprite(data.TexturePath);
//             SpriteSheet sheet = new(sprite, data.Columns, data.Rows, data.SpriteWidth, data.SpriteHeight);

//             _spriteSheets.Add(data.Id, sheet);
//         }
//     }

//     private void LoadCharactersData(string path)
//     {
//         string text = File.ReadAllText(path);
//         CharacterData[] datas = JsonSerializer.Deserialize<CharacterData[]>(text);

//         foreach (CharacterData data in datas)
//         {
//             SpriteSheet 

//             data.IdleAnimations = new AnimationPack(new Dictionary<string, Animation>()
//             {
//                 { "Down", Animation.FromSpriteSheet(sheet, 0) },
//                 { "Left", Animation.FromSpriteSheet(sheet, 4) },
//                 { "Right", Animation.FromSpriteSheet(sheet, 8) },
//                 { "Up", Animation.FromSpriteSheet(sheet, 12) },
//             });

//             data.RunAnimations = new AnimationPack(new Dictionary<string, Animation>()
//             {
//                 { "Down", Animation.FromSpriteSheet(sheet, 0, 3) },
//                 { "Left", Animation.FromSpriteSheet(sheet, 4, 7) },
//                 { "Right", Animation.FromSpriteSheet(sheet, 8, 11) },
//                 { "Up", Animation.FromSpriteSheet(sheet, 12, 15) },
//             });

//             _characters.Add(data.Id, data);
//         }
//     }

//     public void RegisterAsset(string key, Sprite sprite)
//     {
//         if (_sprites.ContainsKey(key))
//             throw new InvalidOperationException($"A sprite with the key '{key}' has already been registered.");

//         _sprites[key] = sprite;
//     }

//     public void RegisterAsset(string key, SpriteSheet spriteSheet)
//     {
//         if (_spriteSheets.ContainsKey(key))
//             throw new InvalidOperationException($"A sprite sheet with the key '{key}' has already been registered.");

//         _spriteSheets[key] = spriteSheet;
//     }

//     public void RegisterAsset(string key, CharacterData character)
//     {
//         if (_characters.ContainsKey(key))
//             throw new InvalidOperationException($"A character with the key '{key}' has already been registered.");

//         _characters[key] = character;
//     }

//     private Sprite LoadSprite(string path)
//     {
//         var texture = _content.Load<Texture2D>(path);
//         return new Sprite(texture, null);
//     }
// }