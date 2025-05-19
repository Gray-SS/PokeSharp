using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PokeSharp.Engine.Assets;
using PokeSharp.Engine.Managers;
using PokeSharp.Engine.UI;
using PokeSharp.ROM;
using PokeSharp.ROM.Events;

namespace PokeSharp.Engine;

public abstract class PokesharpEngine : Game
{
    //Singleton instance
    public static PokesharpEngine Instance { get; private set; } = null!;

    // Services / Managers
    public UIManager UIManager { get; private set; } = null!;
    public RomManager RomManager { get; private set; } = null!;
    public InputManager InputManager { get; private set; } = null!;
    public ScreenManager ScreenManager { get; private set; } = null!;
    public AssetsManager AssetsManager { get; private set; } = null!;
    public WindowManager WindowManager { get; private set; } = null!;
    public CoroutineManager CoroutineManager { get; private set; } = null!;
    public ResolutionManager ResolutionManager { get; private set; } = null!;

    // MonoGame Properties
    public GraphicsDeviceManager Graphics { get; }

    private readonly string _romPath;

    public PokesharpEngine(string romPath)
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

        AssetsManager = new AssetsManager(GraphicsDevice, Content, RomManager);

        UIManager = new UIManager();
        InputManager = new InputManager();
        ScreenManager = new ScreenManager();
        CoroutineManager = new CoroutineManager();
        WindowManager = new WindowManager(Window);
        ResolutionManager = new ResolutionManager(Graphics, Window);

        if (!string.IsNullOrEmpty(_romPath) && RomManager.LoadRom(_romPath))
        {
            PokemonRom rom = RomManager.Rom!;
            WindowManager.SetWindowTitle($"PokéSharp - {rom.Info}");
        }
        else
        {
            WindowManager.SetWindowTitle("PokéSharp - Limited version");
        }

        // ResolutionManager.IsVirtualResEnabled = true;
        ResolutionManager.SetResolution(Resolution.R1280x720);
        // ResolutionManager.SetVirtualResolution(Resolution.R800x400);

        base.Initialize();
    }

    private void OnRomLoadFailed(object? sender, RomLoadFailedArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {e.ErrorMessage}");
        Console.ResetColor();
    }

    private void OnRomLoaded(object? sender, RomLoadedArgs e)
    {
        Console.WriteLine($"ROM loaded successfully: {e.LoadedRom.Info}");
    }

    protected override void LoadContent()
    {
        AssetsManager.LoadGlobalAssets(GraphicsDevice);
        UIManager.Initialize(AssetsManager);
    }

    protected override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        CoroutineManager.Update(dt);

        InputManager.Update();

        if (InputManager.IsKeyPressed(Keys.Escape))
            Exit();

        if (InputManager.IsKeyPressed(Keys.F11))
            ResolutionManager.ToggleFullscreen();

        ScreenManager.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        ScreenManager.Draw(gameTime);
    }
}