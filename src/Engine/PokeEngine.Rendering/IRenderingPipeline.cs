using Microsoft.Xna.Framework;

namespace PokeEngine.Rendering;

public interface IRenderingPipeline
{
    void AddRenderer(IRenderer renderer);

    void RemoveRenderer(IRenderer renderer);

    void Render(GameTime gameTime);
}