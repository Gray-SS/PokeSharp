using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PokeSharp.Engine.Managers;

namespace PokeSharp.Engine.Inputs;

public sealed class InputManager
{
    public Vector2 MousePosition { get; private set; }

    private MouseState _msState;
    private MouseState _lastMsState;

    private KeyboardState _kbState;
    private KeyboardState _lastKbState;
    private ResolutionManager _resManager;

    public InputManager(ResolutionManager resManager)
    {
        _resManager = resManager;
    }

    public void Update()
    {
        _lastKbState = _kbState;
        _lastMsState = _msState;

        _kbState = Keyboard.GetState();
        _msState = Mouse.GetState();

        var mousePos = _msState.Position.ToVector2();
        MousePosition = _resManager.ScreenToGame(mousePos);
    }

    public bool IsMouseButtonDown(MouseButtonType type)
    {
        return type switch
        {
            MouseButtonType.Left => _msState.LeftButton == ButtonState.Pressed,
            MouseButtonType.Middle => _msState.MiddleButton == ButtonState.Pressed,
            MouseButtonType.Right => _msState.RightButton == ButtonState.Pressed,
            MouseButtonType.XButton1 => _msState.XButton1 == ButtonState.Pressed,
            MouseButtonType.XButton2 => _msState.XButton2 == ButtonState.Pressed,
            _ => throw new NotSupportedException($"Mouse button type '{type}' is currently not supported.")
        };
    }

    public bool IsMouseButtonUp(MouseButtonType type)
    {
        return type switch
        {
            MouseButtonType.Left => _msState.LeftButton == ButtonState.Released,
            MouseButtonType.Middle => _msState.MiddleButton == ButtonState.Released,
            MouseButtonType.Right => _msState.RightButton == ButtonState.Released,
            MouseButtonType.XButton1 => _msState.XButton1 == ButtonState.Released,
            MouseButtonType.XButton2 => _msState.XButton2 == ButtonState.Released,
            _ => throw new NotSupportedException($"Mouse button type '{type}' is currently not supported.")
        };
    }

    public bool IsMouseButtonPressed(MouseButtonType type)
    {
        return type switch
        {
            MouseButtonType.Left => _msState.LeftButton == ButtonState.Pressed && _lastMsState.LeftButton == ButtonState.Released,
            MouseButtonType.Middle => _msState.MiddleButton == ButtonState.Pressed && _lastMsState.MiddleButton == ButtonState.Released,
            MouseButtonType.Right => _msState.RightButton == ButtonState.Pressed && _lastMsState.RightButton == ButtonState.Released,
            MouseButtonType.XButton1 => _msState.XButton1 == ButtonState.Pressed && _lastMsState.XButton1 == ButtonState.Released,
            MouseButtonType.XButton2 => _msState.XButton2 == ButtonState.Pressed && _lastMsState.XButton2 == ButtonState.Released,
            _ => throw new NotSupportedException($"Mouse button type '{type}' is currently not supported.")
        };
    }

    public bool IsMouseButtonReleased(MouseButtonType type)
    {
        return type switch
        {
            MouseButtonType.Left => _msState.LeftButton == ButtonState.Released && _lastMsState.LeftButton == ButtonState.Pressed,
            MouseButtonType.Middle => _msState.MiddleButton == ButtonState.Released && _lastMsState.MiddleButton == ButtonState.Pressed,
            MouseButtonType.Right => _msState.RightButton == ButtonState.Released && _lastMsState.RightButton == ButtonState.Pressed,
            MouseButtonType.XButton1 => _msState.XButton1 == ButtonState.Released && _lastMsState.XButton1 == ButtonState.Pressed,
            MouseButtonType.XButton2 => _msState.XButton2 == ButtonState.Released && _lastMsState.XButton2 == ButtonState.Pressed,
            _ => throw new NotSupportedException($"Mouse button type '{type}' is currently not supported.")
        };
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