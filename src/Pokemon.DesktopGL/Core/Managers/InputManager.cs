using Microsoft.Xna.Framework.Input;

namespace Pokemon.DesktopGL.Core.Managers;

public sealed class InputManager
{
    private KeyboardState _kbState;
    private KeyboardState _lastKbState;

    public void Update()
    {
        _lastKbState = _kbState;
        _kbState = Keyboard.GetState();
    }

    public bool IsKeyDown(Keys key)
        => _kbState.IsKeyDown(key);

    public bool IsKeyUp(Keys key)
        => _kbState.IsKeyUp(key);

    public bool IsKeyPressed(Keys key)
        => _kbState.IsKeyDown(key) && _lastKbState.IsKeyUp(key);

    public bool IsKeyReleased(Keys key)
        => _kbState.IsKeyUp(key) && _lastKbState.IsKeyDown(key);
}