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
using Pokemon.DesktopGL.ROM;
using Pokemon.DesktopGL.ROM.Events;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.ConstrainedExecution;
using Pokemon.DesktopGL.ROM.Graphics;

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
    private Texture2D _currentTexture;
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
            PokemonRom rom = RomManager.Rom;
            RomAssetsPack assetPack = rom.ExtractAssetPack();

            EntityGraphicsInfo playerGraphicsInfo = assetPack.PlayerEntityGraphicsInfo;
            _currentTexture = playerGraphicsInfo.SpriteSheet.GenerateTexture(GraphicsDevice);
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

        if (_currentTexture != null)
        {
            Rectangle bounds = new Rectangle(200, 200, 16*9*2, 32*2);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_currentTexture, bounds, Color.White);
            _spriteBatch.End();
        }
    }
}