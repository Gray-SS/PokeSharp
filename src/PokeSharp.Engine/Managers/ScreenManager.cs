using Microsoft.Xna.Framework;

namespace PokeSharp.Engine.Managers;

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
        screen.DoInitialize();
        screen.DoLoad();


        _screens.Push(screen);
    }
    public Screen Pop()
    {
        var screen = _screens.Pop();
        screen.DoUnload();

        _screens.Peek()?.DoLoad();

        return screen;
    }

    public void Update(GameTime gameTime)
    {
        ActiveScreen?.DoUpdate(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        ActiveScreen?.DoDraw(gameTime);
    }
}