using Microsoft.Xna.Framework;
using PokeEngine.Core;
using PokeEngine.Core.Resolutions;

namespace PokeLab.Presentation.ImGui;

public sealed class PokeLabEngine : BaseEngine
{
    private EditorGuiRenderer _renderer = null!;

    public PokeLabEngine(EngineConfiguration config) : base(config)
    {
    }

    protected override void OnInitialize()
    {
        Resolution.SetResolution(ResolutionSize.R1920x1080);
        _renderer = Services.GetService<EditorGuiRenderer>();

        base.OnInitialize();
    }

    protected override void OnLoad()
    {
        base.OnLoad();
    }

    protected override void OnDraw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        base.OnDraw(gameTime);

        _renderer.Draw(gameTime);
    }
}