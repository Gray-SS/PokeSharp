using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PokeSharp.Core.Resolutions;

public static class Resolution
{
    private static readonly IResolutionManager _resManager = ServiceLocator.GetService<IResolutionManager>();

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