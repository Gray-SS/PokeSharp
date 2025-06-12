using Microsoft.Xna.Framework;
using PokeEngine.Core;
using PokeEngine.Core.Resolutions;

namespace PokeLab.Presentation.ImGui;

public sealed class PokeLabEngine : BaseEngine
{
    private PokeLabImGuiRenderer _renderer = null!;

    public PokeLabEngine(EngineConfiguration config) : base(config)
    {
    }

    protected override void OnInitialize()
    {
        Resolution.SetResolution(ResolutionSize.R1280x720);
        _renderer = Services.GetService<PokeLabImGuiRenderer>();

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