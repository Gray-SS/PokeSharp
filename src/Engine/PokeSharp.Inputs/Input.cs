using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PokeSharp.Core;

namespace PokeSharp.Inputs;

public static class Input
{
    private static readonly InputManager _inputManager = S.GetService<InputManager>();

    public static Vector2 GetMousePos()
    {
        return _inputManager.MousePosition;
    }

    public static bool IsKeyDown(Keys key)
    {
        return _inputManager.IsKeyDown(key);
    }

    public static bool IsKeyUp(Keys key)
    {
        return _inputManager.IsKeyUp(key);
    }

    public static bool IsKeyReleased(Keys key)
    {
        return _inputManager.IsKeyReleased(key);
    }

    public static bool IsKeyPressed(Keys key)
    {
        return _inputManager.IsKeyPressed(key);
    }

    public static bool IsMouseButtonDown(MouseButton button)
    {
        return _inputManager.IsMouseButtonDown(button);
    }

    public static bool IsMouseButtonUp(MouseButton button)
    {
        return _inputManager.IsMouseButtonUp(button);
    }

    public static bool IsMouseButtonPressed(MouseButton button)
    {
        return _inputManager.IsMouseButtonPressed(button);
    }

    public static bool IsMouseButtonReleased(MouseButton button)
    {
        return _inputManager.IsMouseButtonReleased(button);
    }
}