using Microsoft.Xna.Framework;

namespace PokeSharp.Core.Services;

public sealed class EngineHookDispatcher : IEngineHookDispatcher
{
    private readonly IEngineHook[] _hooks;

    public EngineHookDispatcher(IReflectionManager reflectionManager)
    {
        _hooks = reflectionManager.InstantiateClassesOfType<IEngineHook>();
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