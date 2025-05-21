using PokeSharp.Core.Services;

namespace PokeSharp.Editor.UI.Services;

public sealed class GuiHookDispatcher : IGuiHookDispatcher
{
    private readonly IGuiHook[] _hooks;

    public GuiHookDispatcher(IReflectionManager reflectionManager)
    {
        _hooks = reflectionManager.InstantiateClassesOfType<IGuiHook>();
    }

    public void Draw()
    {
        foreach (IGuiHook hook in _hooks)
        {
            hook.DrawGui();
        }
    }
}