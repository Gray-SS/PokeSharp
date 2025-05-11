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
using Pokemon.DesktopGL.ROMs;

namespace Pokemon.DesktopGL;

public class PokemonGame : Game
{
    //Singleton instance
    public static PokemonGame Instance { get; private set; }

    // Services / Managers
    public InputManager InputManager { get; private set; }
    public ScreenManager ScreenManager { get; private set; }
    public AssetsManager AssetsManager { get; private set; }
    public WindowManager WindowManager { get; private set; }
    public DialogueManager DialogueManager { get; private set; }
    public CoroutineManager CoroutineManager { get; private set; }

    // Game Properties
    public ROM ROM { get; }
    public Overworld ActiveWorld { get; set; }
    public PlayerData PlayerData { get; set; }
    public MoveRegistry MoveRegistry { get; private set; }
    public CreatureRegistry CreatureRegistry { get; private set; }
    public CharacterRegistry CharacterRegistry { get; private set; }

    // MonoGame Properties
    public GraphicsDeviceManager Graphics { get; }

    public PokemonGame()
    {
        if (Instance != null)
            throw new InvalidOperationException("Impossible to create multiple instances of the game");

        ROM = LoadROM();

        Instance = this;
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
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

        WindowManager.SetWindowTitle("PokéSharp");
        WindowManager.SetResolution("1280x720");

        base.Initialize();
    }

    protected override void LoadContent()
    {
        AssetsManager.LoadGlobalAssets();
        MoveRegistry.Load();
        CreatureRegistry.Load();
        CharacterRegistry.Load();

        Creature creature = CreatureRegistry.GetData("zigzagoon").CreateWild(20);
        PlayerData.AddCreature(creature);

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
    }

    private static ROM LoadROM()
    {
        string[] args = Environment.GetCommandLineArgs();

        if (args.Length < 2)
            throw new NotImplementedException("You MUST pass the ROM path when running the project. Usage: dotnet run <rom_path>");

        string romPath = args[1];
        return ROM.LoadFromPath(romPath);
    }
}
