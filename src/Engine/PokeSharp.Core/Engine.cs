using Microsoft.Xna.Framework;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Services;
using PokeSharp.Core.Attributes;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules;
using Ninject;
using PokeSharp.Core.Windowing;

namespace PokeSharp.Core;

[Priority(-100)]
public abstract class Engine : Game
{
    public IApp App => ServiceLocator.CurrentApp;
    public GraphicsDeviceManager Graphics { get; }

    private readonly IKernel _kernel;
    private readonly ILogger _logger;
    private readonly IModuleLoader _moduleLoader;
    private IEngineHookDispatcher _hooksDispatcher = null!;

    public Engine(EngineConfiguration config)
    {
        ValidateSingleInstance();

        _instance = this;

        // We're not using dependency injection for the engine logger,
        // because injecting it through EngineConfiguration would set its context
        // to EngineConfiguration instead of Engine.
        _logger = LoggerFactory.GetLogger(typeof(Engine));
        _kernel = config.Kernel;
        _moduleLoader = config.ModuleLoader;

        Graphics = new GraphicsDeviceManager(this);

        IsMouseVisible = true;
        Content.RootDirectory = "Content";
    }

    /// <summary>
    /// Cette fonction sert à inject le IEngineHookDispatcher.
    /// </summary>
    /// <param name="dispatcher"></param>
    internal void InjectDispatcher(IEngineHookDispatcher dispatcher)
    {
        _logger.Debug($"{nameof(IEngineHookDispatcher)} has been successfully injected into '{nameof(Engine)}'");
        _hooksDispatcher = dispatcher;
    }

    #region Engine related API

    protected virtual void OnInitialize()
    {
        _hooksDispatcher.Initialize();
    }

    protected virtual void OnLoad()
    {
    }

    protected virtual void OnUpdate(GameTime gameTime)
    {
        _hooksDispatcher.Update(gameTime);
    }

    protected virtual void OnDraw(GameTime gameTime)
    {
        _hooksDispatcher.Draw(gameTime);
    }

    #endregion // Engine related API

    #region Monogame related

    protected sealed override void Initialize()
    {
        LoadModules();

        IWindowManager windowManager = _kernel.Get<IWindowManager>();
        windowManager.Title = $"{App.AppName} - {App.AppVersion}";

        _logger.Info("Initializing the game loop...");
        OnInitialize();
        _logger.Info("Game loop initialized.");

        base.Initialize();
    }

    protected sealed override void LoadContent()
    {
        _logger.Info("Loading content...");
        OnLoad();
        _logger.Info("Content successfully loaded.");
    }

    protected override void BeginRun()
    {
        _logger.Info("Running the game loop...");
    }

    protected sealed override void Update(GameTime gameTime)
    {
        OnUpdate(gameTime);
    }

    protected sealed override void Draw(GameTime gameTime)
    {
        OnDraw(gameTime);
    }

    protected override void OnExiting(object sender, EventArgs args)
    {
        base.OnExiting(sender, args);
        _logger.Info("Exiting the game loop...");
    }

    #endregion // Monogame related API

    #region Helpers

    private void LoadModules()
    {
        _moduleLoader.LoadModules();
        if (!_moduleLoader.IsLoaded)
            throw new AppException("The module loader have not been loaded correctly.");

        _logger.Info($"{_moduleLoader.LoadedModules.Count()} module(s) loaded.");
    }

    private void ValidateSingleInstance()
    {
        if (_instance != null)
        {
            throw new EngineException($"Only one instance of '{nameof(Engine)}' is allowed. " +
                                    $"You tried to create a second instance of '{GetType().Name}'. " +
                                    "Ensure your application only instantiates a single Engine.");
        }
    }

    #endregion

    #region Singleton

    public static Engine Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new EngineException($"""
                    The Engine has not been instantied yet.
                    Make sure to instantiate you Engine subclass before accessing '{nameof(Engine.Instance)}'.
                """);
            }

            return _instance;
        }
    }

    private static Engine _instance = null!;

    #endregion // Singleton
}