using Microsoft.Xna.Framework;

namespace PokeSharp.Core.Windowing.Events;

public sealed class WindowSizeChangedArgs : EventArgs
{
    public Vector2 Size { get; }

    public WindowSizeChangedArgs(Vector2 size)
    {
        Size = size;
    }
}