using Microsoft.Xna.Framework;

namespace PokeSharp.Engine.Managers;

public class WindowManager
{
    public Rectangle Rect => new(0, 0, WindowWidth, WindowHeight);
    public int WindowWidth => _window.ClientBounds.Width;
    public int WindowHeight => _window.ClientBounds.Height;
    public Vector2 WindowSize => _window.ClientBounds.Size.ToVector2();

    private readonly GameWindow _window;

    public WindowManager(GameWindow window)
    {
        ArgumentNullException.ThrowIfNull(window, nameof(window));

        _window = window;
    }

    public float AlignX(float scalar)
        => WindowWidth * scalar;

    public float AlignY(float scalar)
        => WindowHeight * scalar;

    public void SetWindowTitle(string title)
        => _window.Title = title;
}