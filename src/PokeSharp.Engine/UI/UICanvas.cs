using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Engine.Managers;
using PokeSharp.Engine.Renderers;

namespace PokeSharp.Engine.UI;

public sealed class UICanvas
{
    public UINode? Root { get; set; }
    public Viewport Viewport => _resManager.VirtualViewport;

    private readonly ResolutionManager _resManager;

    public UICanvas(ResolutionManager resManager, UINode? root = null)
    {
        ArgumentNullException.ThrowIfNull(resManager, nameof(resManager));

        Root = root;
        _resManager = resManager;
    }

    public void Update(GameTime gameTime)
    {
        Root?.Arrange(Viewport.Bounds);
        Root?.Update(gameTime);
    }

    public void Draw(UIRenderer renderer)
    {
        Root?.Draw(renderer);
    }
}