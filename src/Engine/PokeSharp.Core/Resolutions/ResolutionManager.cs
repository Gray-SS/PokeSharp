using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Core.Resolutions.Events;

namespace PokeSharp.Core.Resolutions;

public sealed class ResolutionManager : IResolutionManager, IDisposable
{
    public bool IsFullScreen { get; }

    public Viewport Viewport => new Viewport
    {
        X = 0,
        Y = 0,
        Width = ResolutionSize.Width,
        Height = ResolutionSize.Height
    };

    public Viewport VirtualViewport => new Viewport
    {
        X = 0,
        Y = 0,
        Width = VirtualResolutionSize.Width,
        Height = VirtualResolutionSize.Height
    };

    public ResolutionSize ResolutionSize => _resolution;
    public ResolutionSize VirtualResolutionSize => _virtualResolution;
    public Matrix VirtualToRealMatrix { get; private set; }
    public Matrix RealToVirtualMatrix { get; private set; }

    public event EventHandler<ResolutionChangedArgs>? ResolutionChanged;
    public event EventHandler<ResolutionChangedArgs>? VirtualResolutionChanged;

    private bool _disposed;
    private bool _virtualResEnabled;
    private ResolutionSize _resolution;
    private ResolutionSize _virtualResolution;

    private readonly GameWindow _window;
    private readonly GraphicsDeviceManager _graphics;

    public static readonly ResolutionSize MinResolution = new(320, 240);
    public static readonly ResolutionSize MaxResolution = new(7680, 4320);

    public ResolutionManager(Engine engine)
    {
        _window = engine.Window;
        _window.ClientSizeChanged += OnClientSizeChanged;

        _graphics = engine.Graphics;
        _resolution = new ResolutionSize(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _virtualResolution = _resolution;
    }

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        var size = _window.ClientBounds.Size;

        ResolutionSize old = _resolution;
        _resolution = new ResolutionSize(size.X, size.Y);

        UpdateTransformMatrices();
        ResolutionChanged?.Invoke(this, new ResolutionChangedArgs(this, _resolution, old));
    }

    private void UpdateTransformMatrices()
    {
        float scaleX = (float)_resolution.Width / _virtualResolution.Width;
        float scaleY = (float)_resolution.Height / _virtualResolution.Height;
        float scale = MathF.Min(scaleX, scaleY);

        float offsetX = (_resolution.Width - (_virtualResolution.Width * scale)) / 2f;
        float offsetY = (_resolution.Height - (_virtualResolution.Height * scale)) / 2f;

        VirtualToRealMatrix =
            Matrix.CreateScale(scale) *
            Matrix.CreateTranslation(offsetX, offsetY, 0f);

        RealToVirtualMatrix = Matrix.Invert(VirtualToRealMatrix);
    }

    public void EnableVirtualResolution(bool enable)
    {
        _virtualResEnabled = enable;

        if (_virtualResEnabled)
        {
            SetVirtualResolution(ResolutionSize);
        }
    }

    public void SetResolution(ResolutionSize resolution)
    {
        if (resolution.Width <= 0 || resolution.Height <= 0)
            throw new InvalidOperationException("Error setting resolution: Resolution dimension must be bigger than 0");

        ValidateResolution(resolution);

        _graphics.PreferredBackBufferWidth = resolution.Width;
        _graphics.PreferredBackBufferHeight = resolution.Height;
        _graphics.ApplyChanges();

        ResolutionSize old = _resolution;
        _resolution = resolution;

        if (!_virtualResEnabled)
            _virtualResolution = resolution;

        UpdateTransformMatrices();
        ResolutionChanged?.Invoke(this, new ResolutionChangedArgs(this, resolution, old));
    }

    public void SetVirtualResolution(ResolutionSize resolution)
    {
        if (!_virtualResEnabled)
            throw new InvalidOperationException("Error setting virtual resolution: Virtual resolution isn't enabled");

        ValidateResolution(resolution);

        ResolutionSize old = _virtualResolution;
        _virtualResolution = resolution;

        UpdateTransformMatrices();
        VirtualResolutionChanged?.Invoke(this, new ResolutionChangedArgs(this, resolution, old));
    }

    private static void ValidateResolution(ResolutionSize resolution)
    {
        if (resolution.Width <= 0 || resolution.Height <= 0)
            throw new InvalidOperationException($"Invalid resolution {resolution}: Width and height must be greater than 0");

        if (resolution.Width < MinResolution.Width || resolution.Height < MinResolution.Height)
            throw new InvalidOperationException($"Invalid resolution {resolution}: Resolution too small (minimum: {MinResolution.Width}x{MinResolution.Height})");

        if (resolution.Width > MaxResolution.Width || resolution.Height > MaxResolution.Height)
            throw new InvalidOperationException($"Invalid resolution {resolution}: Resolution too large (maximum: {MaxResolution.Width}x{MaxResolution.Height})");
    }

    public Vector2 ScreenToVirtual(Vector2 screenPos)
    {
        return Vector2.Transform(screenPos, RealToVirtualMatrix);
    }

    public Vector2 VirtualToScreen(Vector2 virtualPos)
    {
        return Vector2.Transform(virtualPos, VirtualToRealMatrix);
    }

    public bool ToggleFullScreen()
    {
        _graphics.ToggleFullScreen();
        return _graphics.IsFullScreen;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _window.ClientSizeChanged -= OnClientSizeChanged;
            _disposed = true;
        }
    }
}