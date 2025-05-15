using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pokemon.DesktopGL.Characters;
using Pokemon.DesktopGL.Core.Managers;
using Pokemon.DesktopGL.Core.Screens;
using Pokemon.DesktopGL.Dialogues;
using Pokemon.DesktopGL.Creatures;
using Pokemon.DesktopGL.World;
using Pokemon.DesktopGL.Players;
using Pokemon.DesktopGL.Moves;
using Microsoft.Xna.Framework.Graphics;
using Pokemon.DesktopGL.Core.Graphics;
using PokeSharp.ROM;
using PokeSharp.ROM.Graphics;
using PokeSharp.ROM.Events;
using System.Runtime.InteropServices;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

namespace Pokemon.DesktopGL;

public class PokemonGame : Game
{
    //Singleton instance
    public static PokemonGame Instance { get; private set; }

    // Services / Managers
    public RomManager RomManager { get; private set; }
    public InputManager InputManager { get; private set; }
    public ScreenManager ScreenManager { get; private set; }
    public AssetsManager AssetsManager { get; private set; }
    public WindowManager WindowManager { get; private set; }
    public DialogueManager DialogueManager { get; private set; }
    public CoroutineManager CoroutineManager { get; private set; }

    // Game Properties
    public Overworld ActiveWorld { get; set; }
    public PlayerData PlayerData { get; set; }
    public MoveRegistry MoveRegistry { get; private set; }
    public CreatureRegistry CreatureRegistry { get; private set; }
    public CharacterRegistry CharacterRegistry { get; private set; }

    // MonoGame Properties
    public GraphicsDeviceManager Graphics { get; }

    private readonly string _romPath;

    private SpriteSheet _spriteSheet;
    private SpriteBatch _spriteBatch;

    public PokemonGame(string romPath)
    {
        if (Instance != null)
            throw new InvalidOperationException("Impossible to create multiple instances of the game");

        Instance = this;
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _romPath = romPath;
    }

    protected override void Initialize()
    {
        RomManager = new RomManager();
        RomManager.RomLoaded += OnRomLoaded;
        RomManager.RomLoadFailed += OnRomLoadFailed;

        InputManager = new InputManager();
        ScreenManager = new ScreenManager();
        DialogueManager = new DialogueManager();
        CoroutineManager = new CoroutineManager();
        AssetsManager = new AssetsManager(GraphicsDevice, Content);
        WindowManager = new WindowManager(Window, Graphics);

        MoveRegistry = new MoveRegistry();
        CreatureRegistry = new CreatureRegistry(AssetsManager);
        CharacterRegistry = new CharacterRegistry(AssetsManager);

        PlayerData = new PlayerData();

        if (!string.IsNullOrEmpty(_romPath) && RomManager.LoadRom(_romPath))
        {
            PokemonRom rom = RomManager.Rom;
            WindowManager.SetWindowTitle($"PokéSharp - {rom.Info}");
        }
        else
        {
            WindowManager.SetWindowTitle("PokéSharp - Limited version");
        }

        WindowManager.SetResolution("1280x720");
        base.Initialize();
    }

    private void OnRomLoadFailed(object sender, RomLoadFailedArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {e.ErrorMessage}");
        Console.ResetColor();
    }

    private void OnRomLoaded(object sender, RomLoadedArgs e)
    {
        Console.WriteLine($"ROM loaded successfully: {e.LoadedRom.Info}");
    }

    protected override void LoadContent()
    {
        AssetsManager.LoadGlobalAssets();
        MoveRegistry.Load();
        CreatureRegistry.Load();
        CharacterRegistry.Load();

        Creature creature = CreatureRegistry.GetData("zigzagoon").CreateWild(20);
        PlayerData.AddCreature(creature);

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        if (RomManager.IsRomLoaded)
        {
            IPokemonRomProvider provider = RomManager.Rom.Provider;

            EntityGraphicsInfo playerGraphicsInfo = provider.ExtractEntityGraphicsInfo(10);

            RomSpriteSheet spriteSheet = playerGraphicsInfo.SpriteSheet;

            IRomTexture romTexture = spriteSheet.Texture;
            var colors = romTexture.ToRGBA().Select(x => new Color(x.R, x.G, x.B, x.A)).ToArray();

            System.Console.WriteLine($"Alpha 0 pixels ? {(colors.Any(x => x.A == 0) ? "True" : "False")}");

            Texture2D texture = new Texture2D(GraphicsDevice, romTexture.Width, romTexture.Height);
            texture.SetData(colors);

            Sprite sprite = new Sprite(texture);
            _spriteSheet = new SpriteSheet(sprite, spriteSheet.Columns, spriteSheet.Rows, null, null);
        }

        ScreenManager.Push(new OverworldScreen());
    }

    protected override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        CoroutineManager.Update(dt);

        InputManager.Update();
        DialogueManager.Update(dt);

        if (InputManager.IsKeyPressed(Keys.Escape))
            Exit();

        if (InputManager.IsKeyPressed(Keys.F11))
            WindowManager.ToggleFullscreen();

        ScreenManager.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        ScreenManager.Draw(gameTime);

        if (_spriteSheet != null)
        {
            Sprite sprite = _spriteSheet.GetSprite(0);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(sprite.Texture, Vector2.One * 100.0f, sprite.SourceRect, Color.White, 0.0f, Vector2.Zero * 0.5f, 2f, 0, 0.0f);
            _spriteBatch.End();
        }
    }
}