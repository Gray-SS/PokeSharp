using Microsoft.Xna.Framework;
using PokeSharp.Engine;
using PokeSharp.Engine.Renderers;
using PokeSharp.Engine.UI;

namespace Pokemon.DesktopGL.Screens;

public sealed class TitleScreen : Screen
{
    private UICanvas _canvas;
    private UIRenderer _uiRenderer;

    protected override void Initialize()
    {
        _canvas = new UICanvas(ResolutionManager, new UIFrame
        {
        });

        _uiRenderer = new UIRenderer(GraphicsDevice);

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        _canvas.Arrange();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(20, 20, 20));

        _uiRenderer.Begin();
        _canvas.Draw(_uiRenderer);
        _uiRenderer.End();
    }
}