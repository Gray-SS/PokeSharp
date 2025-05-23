using Microsoft.Xna.Framework;
using PokeSharp.Core.Logging;

namespace PokeSharp.Core.Services;

public sealed class EngineHookDispatcher : IEngineHookDispatcher
{
    private readonly ILogger _logger;
    private readonly IEngineHook[] _hooks;

    public EngineHookDispatcher(ILogger logger, IReflectionManager reflectionManager)
    {
        _logger = logger;

        _logger.Debug($"Instantiating engine hooks via '{nameof(IReflectionManager)}'...");
        _hooks = reflectionManager.InstantiateClassesOfType<IEngineHook>();
        logger.Debug($"Successfully instantiated engine hooks.");
    }

    public void Initialize()
    {

        foreach (IEngineHook hook in _hooks)
        {
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