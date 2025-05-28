using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PokeSharp.Assets.VFS;
using PokeSharp.Core;
using PokeSharp.Core.Resolutions;
using PokeSharp.Core.Resolutions.Events;
using PokeSharp.Editor.Services;
using PokeSharp.Inputs;
using PokeSharp.Rendering;

namespace PokeSharp.Editor;

public sealed class EditorEngine : Engine
{
    private RenderTarget2D _renderTarget = null!;
    private IVirtualFileSystem _vfs = null!;
    private EditorGuiRenderer _imGuiRenderer = null!;
    private IEditorProjectManager _projectManager = null!;
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

        _vfs = ServiceLocator.GetService<IVirtualFileSystem>();
        _projectManager = ServiceLocator.GetService<IEditorProjectManager>();
        _projectManager.ProjectOpened += OnProjectOpened;

        base.OnInitialize();
    }

    private void OnProjectOpened(object? sender, EditorProject e)
    {
        var volume = new VolumeInfo("fs", "Local", "Local", FileSystemAccess.All);
        _vfs.MountVolume(volume, new FileSystemProvider(e.ContentDirPath));
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
        // if (Input.IsKeyPressed(Keys.Escape))
        // {
        //     Exit();
        // }

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