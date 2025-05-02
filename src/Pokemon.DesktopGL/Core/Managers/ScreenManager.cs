using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pokemon.DesktopGL.Core.Managers;

public sealed class ScreenManager
{
    public Screen ActiveScreen => _screens.Peek();

    private readonly Stack<Screen> _screens;

    public ScreenManager()
    {
        _screens = new Stack<Screen>();
    }

    public void Push(Screen screen)
    {
        screen.Load();
        _screens.Push(screen);
    }

    public Screen Pop()
    {
        var screen = _screens.Pop();
        screen.Unload();

        return screen;
    }

    public void Update(GameTime gameTime)
    {
        ActiveScreen?.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        ActiveScreen?.Draw(gameTime);
    }
}