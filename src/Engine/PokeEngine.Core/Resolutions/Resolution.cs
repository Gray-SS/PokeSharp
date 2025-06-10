using PokeCore.Hosting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PokeEngine.Core.Resolutions;

public static class Resolution
{
    public static event EventHandler<ResolutionChangedArgs>? ResolutionChanged;
    public static event EventHandler<ResolutionChangedArgs>? VirtualResolutionChanged;

    private static readonly IResolutionManager _resManager;

    static Resolution()
    {
        _resManager = ServiceLocator.GetService<IResolutionManager>();
        _resManager.ResolutionChanged += OnResolutionChanged;
        _resManager.VirtualResolutionChanged += OnVirtualResolutionChanged;
    }

    private static void OnVirtualResolutionChanged(object? sender, ResolutionChangedArgs e)
    {
        ResolutionChanged?.Invoke(sender, e);
    }

    private static void OnResolutionChanged(object? sender, ResolutionChangedArgs e)
    {
        VirtualResolutionChanged?.Invoke(sender, e);
    }

    public static Viewport Viewport => _resManager.Viewport;
    public static Viewport VirtualViewport => _resManager.VirtualViewport;

    public static ResolutionSize ResolutionSize => _resManager.ResolutionSize;
    public static ResolutionSize VirtualResolutionSize => _resManager.VirtualResolutionSize;

    public static void EnableVirtualResolution(bool enable)
    {
        _resManager.EnableVirtualResolution(enable);
    }

    public static void SetResolution(ResolutionSize resolution)
    {
        _resManager.SetResolution(resolution);
    }

    public static void SetVirtualResolution(ResolutionSize resolution)
    {
        _resManager.SetVirtualResolution(resolution);
    }

    public static Vector2 ScreenToVirtual(Vector2 screenPos)
    {
        return _resManager.ScreenToVirtual(screenPos);
    }

    public static Vector2 VirtualToScreen(Vector2 virtualPos)
    {
        return _resManager.VirtualToScreen(virtualPos);
    }

    public static void ToggleFullScreen()
    {
        _resManager.ToggleFullScreen();
    }
}