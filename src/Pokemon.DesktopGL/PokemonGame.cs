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

    private float _animTimer;
    private int _crntAnimIndex;
    private Animation[] _animations;
    private AnimationPlayer _animPlayer;

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
            EntityGraphicsInfo info = RomManager.Rom.Provider.Load(assetsPack.EntitiesGraphicsInfo[49]);

            IRomTexture romTexture = info.SpriteSheet.Texture;
            var pixelsData = romTexture.ToRGBA().Select(x => new Color(x.R, x.G, x.B, x.A)).ToArray();

            Texture2D texture = new Texture2D(GraphicsDevice, romTexture.Width, romTexture.Height);
            texture.SetData(pixelsData);

            _sprite = new Sprite(texture);
            _spriteSheet = new SpriteSheet(_sprite, info.SpriteSheet.Columns, info.SpriteSheet.Rows, null, null);

            var frames = _spriteSheet.Sprites.ToList();

            _animations = new Animation[info.Animations.Length];
            for (int i = 0; i < info.Animations.Length; i++)
            {
                var romAnim = info.Animations[i];
                List<IAnimationCmd> animCmds = [.. romAnim.Commands.Select<object, IAnimationCmd>(x =>
                {
                    return x switch
                    {
                        RomAnimationCmdFrame cmdFrame => new AnimationCmdFrame(cmdFrame.Index, cmdFrame.Duration, cmdFrame.HFlip, cmdFrame.VFlip),
                        RomAnimationCmdJump cmdJump => new AnimationCmdJump(cmdJump.Target),
                        RomAnimationCmdLoop cmdLoop => new AnimationCmdLoop(cmdLoop.Count),
                        RomAnimationCmdEnd => new AnimationCmdEnd(),
                        _ => throw new NotImplementedException("Cmd not implemented")
                    };
                })];

                _animations[i] = new Animation(frames, animCmds);
            }

            _animTimer = 0.0f;
            _animPlayer = new AnimationPlayer();
            _animPlayer.Play(_animations[_crntAnimIndex]);
        }

        ScreenManager.Push(new OverworldScreen());
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _animTimer += dt;
        if (_animTimer >= 2f)
        {
            _animTimer -= 2f;
            _crntAnimIndex = (_crntAnimIndex + 1) % _animations.Length;

            _animPlayer.Play(_animations[_crntAnimIndex]);
        }

        _animPlayer.Update(dt);

        DialogueManager.Update(dt);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        if (_animations != null)
        {
            Sprite sprite = _animPlayer.Frame;
            SpriteEffects effect = _animPlayer.FlipH ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(sprite.Texture, Vector2.One * 100.0f, sprite.SourceRect, Color.White, 0.0f, Vector2.Zero * 0.5f, 10f, effect, 0.0f);
            _spriteBatch.End();
        }
    }
}