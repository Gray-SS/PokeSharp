using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Dialogues;
using Pokemon.DesktopGL.Creatures;
using Pokemon.DesktopGL.World;
using Pokemon.DesktopGL.Players;
using Pokemon.DesktopGL.Moves;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Engine.Graphics;
using PokeSharp.Engine;
using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Screens;
using PokeSharp.ROM;
using System;
using PokeSharp.ROM.Graphics;
using System.Linq;

namespace Pokemon.DesktopGL;

public class PokemonGame : PokesharpEngine
{
    public new static PokemonGame Instance => (PokemonGame)PokesharpEngine.Instance;

    // Game Properties
    public DialogueManager DialogueManager { get; private set; }
    public Overworld ActiveWorld { get; set; }
    public PlayerData PlayerData { get; set; }
    public MoveRegistry MoveRegistry { get; private set; }
    public CreatureRegistry CreatureRegistry { get; private set; }
    public CharacterRegistry CharacterRegistry { get; private set; }

    private SpriteSheet _spriteSheet;
    private SpriteBatch _spriteBatch;
    private Sprite _sprite;

    public PokemonGame(string romPath) : base(romPath)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        DialogueManager = new DialogueManager();
        MoveRegistry = new MoveRegistry();
        CreatureRegistry = new CreatureRegistry(AssetsManager);
        CharacterRegistry = new CharacterRegistry(AssetsManager);

        PlayerData = new PlayerData();

        MoveRegistry.Load();
        CreatureRegistry.Load();
        CharacterRegistry.Load();

        Creature creature = CreatureRegistry.GetData("zigzagoon").CreateWild(20);
        PlayerData.AddCreature(creature);

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        if (RomManager.IsRomLoaded)
        {
            RomAssetsPack assetsPack = RomManager.Rom.ExtractAssetPack();
            Console.WriteLine(RomManager.Rom.Provider.Load(assetsPack.PokemonNames[49]));

            IRomTexture romTexture = RomManager.Rom.Load(assetsPack.PokemonBackSprites[49]);
            var pixelsData = romTexture.ToRGBA().Select(x => new Color(x.R, x.G, x.B, x.A)).ToArray();

            Texture2D texture = new Texture2D(GraphicsDevice, romTexture.Width, romTexture.Height);
            texture.SetData(pixelsData);

            _sprite = new Sprite(texture);
        }

        ScreenManager.Push(new OverworldScreen());
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        DialogueManager.Update(dt);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        if (_spriteSheet != null)
        {
            Sprite sprite = _spriteSheet.GetSprite(0);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(sprite.Texture, Vector2.One * 100.0f, sprite.SourceRect, Color.White, 0.0f, Vector2.Zero * 0.5f, 2f, 0, 0.0f);
            _spriteBatch.End();
        }

        if (_sprite != null)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_sprite.Texture, Vector2.One * 100, _sprite.SourceRect, Color.White, 0.0f, Vector2.Zero, 2f, 0, 0);
            _spriteBatch.End();
        }
    }
}