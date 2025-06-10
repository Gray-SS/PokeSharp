using Microsoft.Xna.Framework;
using PokeCore.Hosting.Logging;
using PokeCore.Hosting.Services;

namespace PokeEngine.Core.Services;

public sealed class EngineHookDispatcher : IEngineHookDispatcher
{
    private readonly Logger _logger;
    private readonly IEngineHook[] _hooks;

    public EngineHookDispatcher(Logger logger, IReflectionManager reflectionManager)
    {
        _logger = logger;
        _hooks = reflectionManager.InstantiateClassesOfType<IEngineHook>();

        if (_hooks.Length == 0)
            _logger.Warn("No engine hooks have been found. Please ensure you've loaded your modules correcty.");
    }

    public void Initialize()
    {
        if (_hooks.Length == 0)
        {
            _logger.Warn("No engine hook to initialize.");
            return;
        }

        _logger.Debug("Initializing engine hooks...");
        foreach (IEngineHook hook in _hooks)
        {
            _logger.Trace($"Initializing engine hook: {hook.GetType().Name}");
            hook.Initialize();
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (IEngineHook hook in _hooks)
        {
            hook.Update(gameTime);
        }
    }

    public void Draw(GameTime gameTime)
    {
        foreach (IEngineHook hook in _hooks)
        {
            hook.Draw(gameTime);
        }
    }
}