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
using PokeSharp.ROM.Platforms.GBA.Providers;
using PokeSharp.Engine.Graphics.Animations;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using PokeSharp.Engine.Assets;
using PokeSharp.ROM.Descriptors;

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

    // private float _animTimer;
    // private int _crntAnimIndex;
    // private Animation[] _animations;
    // private AnimationPlayer _animPlayer;

    // private SpriteSheet _spriteSheet;
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

        ScreenManager.Push(new TitleScreen());
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
    }
}