using PokeEngine.Core;
using Microsoft.Xna.Framework;

namespace PokeLab.Presentation.ImGui;

public sealed class ImGuiTickSource : BaseEngine, ITickSource
{
    public event Action? OnTick;

    public ImGuiTickSource(EngineConfiguration config) : base(config)
    {
    }

    protected override void OnDraw(GameTime gameTime)
    {
        OnTick?.Invoke();
        base.OnDraw(gameTime);
    }
}