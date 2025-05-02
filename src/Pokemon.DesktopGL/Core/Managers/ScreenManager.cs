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
        _screens.Push(screen);
    }

    public Screen Pop()
    {
        return _screens.Pop();
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