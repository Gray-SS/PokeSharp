using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PokeSharp.Core.Resolutions.Events;

namespace PokeSharp.Core.Resolutions;

public interface IResolutionManager
{
    bool IsFullScreen { get; }

    Viewport Viewport { get; }
    Viewport VirtualViewport { get; }

    ResolutionSize ResolutionSize { get; }
    ResolutionSize VirtualResolutionSize { get; }

    Matrix VirtualToRealMatrix { get; }
    Matrix RealToVirtualMatrix { get; }

    event EventHandler<ResolutionChangedArgs>? ResolutionChanged;
    event EventHandler<ResolutionChangedArgs>? VirtualResolutionChanged;

    public static readonly ResolutionSize MinResolution = new(320, 240);
    public static readonly ResolutionSize MaxResolution = new(7680, 4320);

    void EnableVirtualResolution(bool enable);

    void SetResolution(ResolutionSize resolution);
    void SetVirtualResolution(ResolutionSize resolution);

    Vector2 ScreenToVirtual(Vector2 screenPos);
    Vector2 VirtualToScreen(Vector2 virtualPos);

    bool ToggleFullScreen();
}