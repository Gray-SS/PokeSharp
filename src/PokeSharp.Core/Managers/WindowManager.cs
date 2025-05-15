using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PokeSharp.Core.Managers;

public class WindowManager
{
    public Rectangle Rect => new(0, 0, WindowWidth, WindowHeight);
    public int WindowWidth => _graphics.PreferredBackBufferWidth;
    public int WindowHeight => _graphics.PreferredBackBufferHeight;
    public Vector2 WindowSize => new(WindowWidth, WindowHeight);

    private readonly GameWindow _window;
    private readonly GraphicsDeviceManager _graphics;

    private readonly Dictionary<string, (int Width, int Height)> _resolutions = new Dictionary<string, (int Width, int Height)>()
    {
        { "800x400", (800, 400) },
        { "1280x720", (1280, 720) },
    };

    public WindowManager(GameWindow window, GraphicsDeviceManager graphics)
    {
        _window = window;
        _graphics = graphics;
    }

    public float AlignX(float scalar)
        => WindowWidth * scalar;

    public float AlignY(float scalar)
        => WindowHeight * scalar;

    public void SetWindowTitle(string title)
        => _window.Title = title;

    public void SetResolution(string resolutionKey)
    {
        if (!_resolutions.TryGetValue(resolutionKey, out var result))
            throw new InvalidOperationException($"Couldn't find a resolution with key '{resolutionKey}'");

        _graphics.PreferredBackBufferWidth = result.Width;
        _graphics.PreferredBackBufferHeight = result.Height;
        _graphics.ApplyChanges();
    }

    public void ToggleFullscreen()
        => _graphics.ToggleFullScreen();
}