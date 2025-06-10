using Microsoft.Xna.Framework;

namespace PokeEngine.Rendering;

public sealed class RenderingPipeline : IRenderingPipeline
{
    private readonly List<IRenderer> _renderers;

    public RenderingPipeline()
    {
        _renderers = new List<IRenderer>();
    }

    public void AddRenderer(IRenderer renderer)
    {
        _renderers.Add(renderer);
    }

    public void RemoveRenderer(IRenderer renderer)
    {
        _renderers.Remove(renderer);
    }

    public void Render(GameTime gameTime)
    {
        foreach (IRenderer renderer in _renderers)
        {
            renderer.Draw(gameTime);
        }
    }
}