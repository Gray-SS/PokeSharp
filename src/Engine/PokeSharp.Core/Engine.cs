using Ninject;
using Ninject.Infrastructure;
using Microsoft.Xna.Framework;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Services;
using PokeSharp.Core.Extensions;
using PokeSharp.Core.Attributes;

namespace PokeSharp.Core;

[Priority(999)]
public abstract class Engine : Game, IHaveKernel
{
    public IKernel Kernel { get; }
    public GraphicsDeviceManager Graphics { get; }

    private IEngineHookDispatcher _hooksDispatcher = null!;

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

    protected abstract void LoadModules(IKernel kernel);

    protected sealed override void Initialize()
    {
        base.Initialize();
    }

    protected sealed override void LoadContent()
    {
        this.LoadCoreModule();
        this.LoadModules(Kernel);

        _hooksDispatcher = Kernel.Get<IEngineHookDispatcher>();
        _hooksDispatcher.Initialize();
    }

    protected sealed override void Update(GameTime gameTime)
    {
        _hooksDispatcher.Update(gameTime);
    }

    protected sealed override void Draw(GameTime gameTime)
    {
        _hooksDispatcher.Draw(gameTime);
    }

    protected virtual void LoadCoreModule()
    {
        Kernel.LoadCoreModule();
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

public abstract class Engine<TSelf> : Engine where TSelf : Engine<TSelf>
{
    protected sealed override void LoadCoreModule()
    {
        Kernel.LoadCoreModule<TSelf>();
    }
}