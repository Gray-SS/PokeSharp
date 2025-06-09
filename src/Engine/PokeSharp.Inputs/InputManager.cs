using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PokeSharp.Core.Annotations;
using PokeSharp.Engine.Core;
using PokeSharp.Engine.Core.Resolutions;

namespace PokeSharp.Inputs;

[Priority(999)]
public sealed class InputManager : IInputManager, IEngineHook
{
    public Vector2 MousePosition { get; private set; }

    private MouseState _msState;
    private MouseState _lastMsState;

    private KeyboardState _kbState;
    private KeyboardState _lastKbState;
    private IResolutionManager _resManager;

    public InputManager(IResolutionManager resManager)
    {
        _resManager = resManager;
    }

    public void Initialize()
    {
    }

    public void Draw(GameTime gameTime)
    {
    }

    public void Update(GameTime gameTime)
    {
        _lastKbState = _kbState;
        _lastMsState = _msState;

        _kbState = Keyboard.GetState();
        _msState = Mouse.GetState();

        var mousePos = _msState.Position.ToVector2();
        MousePosition = _resManager.VirtualToScreen(mousePos);
    }

    public bool IsMouseButtonDown(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => _msState.LeftButton == ButtonState.Pressed,
            MouseButton.Middle => _msState.MiddleButton == ButtonState.Pressed,
            MouseButton.Right => _msState.RightButton == ButtonState.Pressed,
            MouseButton.XButton1 => _msState.XButton1 == ButtonState.Pressed,
            MouseButton.XButton2 => _msState.XButton2 == ButtonState.Pressed,
            _ => throw new NotSupportedException($"Mouse button type '{button}' is currently not supported.")
        };
    }

    public bool IsMouseButtonUp(MouseButton button)
    {
        return button switch
        {
            MouseButton.Middle => _msState.MiddleButton == ButtonState.Released,
            MouseButton.Left => _msState.LeftButton == ButtonState.Released,
            MouseButton.Right => _msState.RightButton == ButtonState.Released,
            MouseButton.XButton1 => _msState.XButton1 == ButtonState.Released,
            MouseButton.XButton2 => _msState.XButton2 == ButtonState.Released,
            _ => throw new NotSupportedException($"Mouse button type '{button}' is currently not supported.")
        };
    }

    public bool IsMouseButtonPressed(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => _msState.LeftButton == ButtonState.Pressed && _lastMsState.LeftButton == ButtonState.Released,
            MouseButton.Middle => _msState.MiddleButton == ButtonState.Pressed && _lastMsState.MiddleButton == ButtonState.Released,
            MouseButton.Right => _msState.RightButton == ButtonState.Pressed && _lastMsState.RightButton == ButtonState.Released,
            MouseButton.XButton1 => _msState.XButton1 == ButtonState.Pressed && _lastMsState.XButton1 == ButtonState.Released,
            MouseButton.XButton2 => _msState.XButton2 == ButtonState.Pressed && _lastMsState.XButton2 == ButtonState.Released,
            _ => throw new NotSupportedException($"Mouse button type '{button}' is currently not supported.")
        };
    }

    public bool IsMouseButtonReleased(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => _msState.LeftButton == ButtonState.Released && _lastMsState.LeftButton == ButtonState.Pressed,
            MouseButton.Middle => _msState.MiddleButton == ButtonState.Released && _lastMsState.MiddleButton == ButtonState.Pressed,
            MouseButton.Right => _msState.RightButton == ButtonState.Released && _lastMsState.RightButton == ButtonState.Pressed,
            MouseButton.XButton1 => _msState.XButton1 == ButtonState.Released && _lastMsState.XButton1 == ButtonState.Pressed,
            MouseButton.XButton2 => _msState.XButton2 == ButtonState.Released && _lastMsState.XButton2 == ButtonState.Pressed,
            _ => throw new NotSupportedException($"Mouse button type '{button}' is currently not supported.")
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