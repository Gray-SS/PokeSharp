using Ninject;
using Microsoft.Xna.Framework;
using PokeSharp.Core.Exceptions;
using PokeSharp.Core.Services;
using PokeSharp.Core.Resolutions;

namespace PokeSharp.Core;

public abstract class Engine : Game
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

    protected sealed override void Initialize()
    {
        base.Initialize();
    }

    protected sealed override void LoadContent()
    {
        Kernel.Load("*.dll");

        _hooksDispatcher = Kernel.Get<IEngineHookDispatcher>();
        _hooksDispatcher.Initialize();
    }

    protected sealed override void Update(GameTime gameTime)
    {
        _hooksDispatcher.Update(gameTime);
    }

    protected sealed override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _hooksDispatcher.Draw(gameTime);
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