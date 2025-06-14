using Microsoft.Xna.Framework;
using PokeCore.Logging;
using PokeCore.Hosting.Abstractions;
using PokeCore.DependencyInjection.Abstractions;
using PokeEngine.Core.Windowing;
using PokeEngine.Core.Threadings;

namespace PokeEngine.Core;

public abstract class BaseEngine : Game, IGameLoop
{
    public GraphicsDeviceManager Graphics { get; }

    public IApp App { get; }
    public Logger Logger { get; }
    public new IServiceContainer Services { get; }

    public event EventHandler? Initialized;
    public event EventHandler<DrawContext>? Rendered;
    public event EventHandler<UpdateContext>? Updated;

    private readonly DrawContext _drawContext;
    private readonly UpdateContext _updateContext;

    // private IEngineHookDispatcher _hooksDispatcher = null!;

    public BaseEngine(EngineConfiguration config)
    {
        App = config.App;
        Logger = config.Logger;
        Services = config.Services;
        Graphics = new GraphicsDeviceManager(this);

        IsMouseVisible = true;
        Content.RootDirectory = "Content";

        _drawContext = new DrawContext();
        _updateContext = new UpdateContext();
    }

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
        Initialized?.Invoke(this, EventArgs.Empty);

        // _hooksDispatcher = Services.GetService<IEngineHookDispatcher>();
        // _hooksDispatcher.Initialize();
    }

    /// <summary>
    /// Used to load any data specific to the child engine
    /// </summary>
    protected virtual void OnLoad()
    {
    }

    protected virtual void OnUpdate(GameTime gameTime)
    {
        _updateContext.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _updateContext.TotalTime = (float)gameTime.TotalGameTime.TotalSeconds;

        Updated?.Invoke(this, _updateContext);
        // _hooksDispatcher.Update(gameTime);
    }

    protected virtual void OnDraw(GameTime gameTime)
    {
        _drawContext.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Rendered?.Invoke(this, _drawContext);
        // _hooksDispatcher.Draw(gameTime);
    }

    protected sealed override void Initialize()
    {
        ThreadHelper.Initialize();

        // LoadModules();
        ConfigureWindowTitle();

        Logger.Info("Initializing the game loop...");
        OnInitialize();

        base.Initialize();
    }

    protected sealed override void LoadContent()
    {
        OnLoad();
    }

    protected override void BeginRun()
    {
        Logger.Info("Entering the game loop");
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
        Logger.Info("Exiting the game loop");
    }

    private void ConfigureWindowTitle()
    {
        IWindowManager windowManager = Services.GetService<IWindowManager>();
        windowManager.Title = $"{App.AppName} - {App.AppVersion}";
    }
}