using PokeEngine.Core;

namespace PokeLab.Presentation.ImGui;

public sealed class ImGuiTickSource : ITickSource
{
    public event Action? OnTick;

    public ImGuiTickSource(IGameLoop gameLoop)
    {
        gameLoop.Rendered += OnDraw;
    }

    private void OnDraw(object? sender, DrawContext context)
    {
        OnTick?.Invoke();
    }
}