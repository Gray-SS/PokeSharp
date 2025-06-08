using Microsoft.Xna.Framework;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Services;
using PokeSharp.Core.Annotations;
using PokeSharp.Core.Logging;
using PokeSharp.Core.Modules;
using Ninject;
using PokeSharp.Core.Windowing;
using PokeSharp.Core.Threadings;

namespace PokeSharp.Core;

[Priority(-100)]
public abstract class Engine : Game
{
    public IApp App => ServiceLocator.CurrentApp;
    public GraphicsDeviceManager Graphics { get; }
    public Logger Logger => _logger;
    public IKernel Kernel => _kernel;
    public IModuleLoader ModuleLoader => _moduleLoader;

    private readonly IKernel _kernel;
    private readonly Logger _logger;
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
    /// This function is used to inject the IEngineHookDispatcher from the core module.
    /// </summary>
    /// <remarks>
    /// Why not retrieve it directly via dependency injection? Because the IEngineHookDispatcher loads IEngineHooks, which are essentially services using MonoGame graphics.
    /// If we inject it directly, then dependencies such as GraphicsDevice, GameWindow, etc. won't be accessible.
    /// </remarks>
    /// <param name="dispatcher">The resolved dispatcher</param>
    internal void InjectDispatcher(IEngineHookDispatcher dispatcher)
    {
        _logger.Debug($"{nameof(IEngineHookDispatcher)} has been successfully injected into '{nameof(Engine)}'");
        _hooksDispatcher = dispatcher;
    }

    #region Engine related API

    /// <summary>
    /// Used to initialize graphics.
    /// </summary>
    /// <remarks>
    /// This function is called when Monogame graphics are initialized. The <b>engine hook dispatcher</b> is called in this function to <b>initialize</b> all the engine hooks.
    /// This is done so that child engines can have <b>full control over their initialization</b>.
    /// However, not calling <see cref="base.Initialize()"/> can render modules non-functional and <b>may lead to exceptions</b>.
    /// </remarks>
    protected virtual void OnInitialize()
    {
        _hooksDispatcher.Initialize();
    }

    /// <summary>
    /// Used to load any data specific to the child engine
    /// </summary>
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
        ThreadHelper.Initialize();

        LoadModules();
        ConfigureWindowTitle();

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
        ThreadHelper.Update();

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

    private void ConfigureWindowTitle()
    {
        IWindowManager windowManager = _kernel.Get<IWindowManager>();
        windowManager.Title = $"{App.AppName} - {App.AppVersion}";
    }

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

    // With the refactor and the 

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