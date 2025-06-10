using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PokeCore.Hosting;
using PokeEngine.Core;
using PokeEngine.Core.Resolutions;
using PokeEngine.Inputs;
using PokeEngine.Rendering;

namespace PokeLab;

public sealed class EditorEngine : BaseEngine
{
    private RenderTarget2D _renderTarget = null!;
    private EditorGuiRenderer _imGuiRenderer = null!;
    private IRenderingPipeline _renderingPipeline = null!;

    public EditorEngine(EngineConfiguration config) : base(config)
    {
        Window.AllowUserResizing = true;
    }

    protected override void OnInitialize()
    {
        Resolution.ResolutionChanged += OnResolutionChanged;
        Resolution.SetResolution(ResolutionSize.R1920x1080);

        _imGuiRenderer = ServiceLocator.GetService<EditorGuiRenderer>();
        _renderingPipeline = ServiceLocator.GetService<IRenderingPipeline>();

        base.OnInitialize();
    }

    protected override void OnLoad()
    {
        _renderingPipeline.AddRenderer(_imGuiRenderer);

        base.OnLoad();
    }

    private void OnResolutionChanged(object? sender, ResolutionChangedArgs e)
    {
        ResolutionSize res = e.NewResolution;

        _renderTarget?.Dispose();
        _renderTarget = new RenderTarget2D(GraphicsDevice, res.Width, res.Height);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (Input.IsKeyPressed(Keys.F11))
        {
            Resolution.ToggleFullScreen();
        }

        base.OnUpdate(gameTime);
    }

    protected override void OnDraw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Transparent);

        base.OnDraw(gameTime);

        _renderingPipeline.Render(gameTime);
    }
}