using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PokeEngine.Inputs;

public interface IInputManager
{
    Vector2 MousePosition { get; }

    bool IsMouseButtonUp(MouseButton button);
    bool IsMouseButtonDown(MouseButton button);
    bool IsMouseButtonPressed(MouseButton button);
    bool IsMouseButtonReleased(MouseButton button);

    bool IsKeyDown(Keys key);
    bool IsKeyUp(Keys key);
    bool IsKeyPressed(Keys key);
    bool IsKeyReleased(Keys key);
}