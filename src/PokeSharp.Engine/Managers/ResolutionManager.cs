using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PokeSharp.Engine.Managers;

public sealed class ResolutionManager : IDisposable
{
    public bool IsVirtualResEnabled { get; set; }

    public Viewport Viewport => new Viewport
    {
        X = 0,
        Y = 0,
        Width = Resolution.Width,
        Height = Resolution.Height
    };

    public Viewport VirtualViewport => new Viewport
    {
        X = 0,
        Y = 0,
        Width = VirtualResolution.Width,
        Height = VirtualResolution.Height
    };

    public Resolution Resolution => _resolution;
    public Resolution VirtualResolution => _virtualResolution;
    public Matrix VirtualToRealMatrix { get; private set; }
    public Matrix RealToVirtualMatrix { get; private set; }

    public event EventHandler<ResolutionChangedArgs>? ResolutionChanged;
    public event EventHandler<ResolutionChangedArgs>? VirtualResolutionChanged;

    private bool _disposed;
    private Resolution _resolution;
    private Resolution _virtualResolution;

    private readonly GameWindow _window;
    private readonly GraphicsDeviceManager _graphics;

    public static readonly Resolution MinResolution = new(320, 240);
    public static readonly Resolution MaxResolution = new(7680, 4320);

    public ResolutionManager(GraphicsDeviceManager graphics, GameWindow window)
    {
        ArgumentNullException.ThrowIfNull(window, nameof(window));
        ArgumentNullException.ThrowIfNull(graphics, nameof(graphics));

        _window = window;
        _window.ClientSizeChanged += OnClientSizeChanged;

        _graphics = graphics;
        _resolution = new Resolution(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _virtualResolution = _resolution;
    }

    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        var size = _window.ClientBounds.Size;

        Resolution old = _resolution;
        _resolution = new Resolution(size.X, size.Y);

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

    public void SetResolution(Resolution resolution)
    {
        if (resolution.Width <= 0 || resolution.Height <= 0)
            throw new InvalidOperationException("Error setting resolution: Resolution dimension must be bigger than 0");

        ValidateResolution(resolution);

        _graphics.PreferredBackBufferWidth = resolution.Width;
        _graphics.PreferredBackBufferHeight = resolution.Height;
        _graphics.ApplyChanges();

        Resolution old = _resolution;
        _resolution = resolution;

        if (!IsVirtualResEnabled)
            _virtualResolution = resolution;

        UpdateTransformMatrices();
        ResolutionChanged?.Invoke(this, new ResolutionChangedArgs(this, resolution, old));
    }

    public void SetVirtualResolution(Resolution resolution)
    {
        if (!IsVirtualResEnabled)
            throw new InvalidOperationException("Error setting virtual resolution: Virtual resolution isn't enabled");

        ValidateResolution(resolution);

        Resolution old = _virtualResolution;
        _virtualResolution = resolution;

        UpdateTransformMatrices();
        VirtualResolutionChanged?.Invoke(this, new ResolutionChangedArgs(this, resolution, old));
    }

    private static void ValidateResolution(Resolution resolution)
    {
        if (resolution.Width <= 0 || resolution.Height <= 0)
            throw new InvalidOperationException($"Invalid resolution {resolution}: Width and height must be greater than 0");

        if (resolution.Width < MinResolution.Width || resolution.Height < MinResolution.Height)
            throw new InvalidOperationException($"Invalid resolution {resolution}: Resolution too small (minimum: {MinResolution.Width}x{MinResolution.Height})");

        if (resolution.Width > MaxResolution.Width || resolution.Height > MaxResolution.Height)
            throw new InvalidOperationException($"Invalid resolution {resolution}: Resolution too large (maximum: {MaxResolution.Width}x{MaxResolution.Height})");
    }

    public Vector2 ScreenToGame(Vector2 position)
    {
        return Vector2.Transform(position, RealToVirtualMatrix);
    }

    public Vector2 GameToScreen(Vector2 position)
    {
        return Vector2.Transform(position, VirtualToRealMatrix);
    }

    public void ToggleFullscreen()
    {
        _graphics.ToggleFullScreen();
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

public sealed class ResolutionChangedArgs : EventArgs
{
    public ResolutionManager ResolutionManager { get; }
    public Resolution NewResolution { get; }
    public Resolution OldResolution { get; }

    public ResolutionChangedArgs(ResolutionManager resolutionManager, Resolution newResolution, Resolution oldResolution)
    {
        ArgumentNullException.ThrowIfNull(resolutionManager, nameof(resolutionManager));

        ResolutionManager = resolutionManager;
        NewResolution = newResolution;
        OldResolution = oldResolution;
    }
}

public readonly struct Resolution : IEquatable<Resolution>
{
    public int Width { get; }
    public int Height { get; }

    public float AspectRatio => (float)Width / Height;
    public bool IsLandscape => Width > Height;
    public bool IsPortrait => Height > Width;
    public bool IsSquare => Width == Height;

    public static readonly Resolution R800x400 = new(800, 400);
    public static readonly Resolution R1280x720 = new(1280, 720);
    public static readonly Resolution R1920x1080 = new(1920, 1080);
    public static readonly Resolution R3840x2160 = new(3840, 2160);
    public static readonly Resolution R2560x1440 = new(2560, 1440);

    public Resolution(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public bool Equals(Resolution other)
    {
        return Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object? obj)
    {
        return obj is Resolution resolution && Equals(resolution);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }

    public static bool operator ==(Resolution left, Resolution right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Resolution left, Resolution right)
    {
        return !left.Equals(right);
    }
}