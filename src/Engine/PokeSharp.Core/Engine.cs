using Ninject;
using Microsoft.Xna.Framework;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Services;

namespace PokeSharp.Core;

public abstract class Engine : Game
{
    public IKernel Kernel { get; }
    public GraphicsDeviceManager Graphics { get; }

    private IEngineHook[] _hooks = null!;

    public Engine()
    {
        if (_instance != null)
        {
            throw new EngineException($"""
                Only one instance of '{nameof(Engine)}' is allowed.
                You tried to create a second instance of '{GetType().Name}.'
                Ensure your application only instantiates a single Engine.
            """);
        }

        _instance = this;

        Kernel = new StandardKernel();
        Graphics = new GraphicsDeviceManager(this);

        IsMouseVisible = true;
        Content.RootDirectory = "Content";
    }

    protected sealed override void Initialize()
    {
        base.Initialize();
    }

    protected sealed override void LoadContent()
    {
        Kernel.Load("*.dll");
        _hooks = Kernel.Get<ReflectionManager>().InstantiateClassesOfType<IEngineHook>();

        ResolutionManager resolutionManager = Kernel.Get<ResolutionManager>();
        resolutionManager.SetResolution(Resolution.R1280x720);

        foreach (IEngineHook hook in _hooks)
        {
            hook.Initialize();
        }
    }

    protected sealed override void Update(GameTime gameTime)
    {
        foreach (IEngineHook hook in _hooks)
        {
            hook.Update(gameTime);
        }
    }

    protected sealed override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        foreach (IEngineHook hook in _hooks)
        {
            hook.Draw(gameTime);
        }
    }

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