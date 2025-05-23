using Microsoft.Xna.Framework;
using PokeSharp.Core.Windowing.Events;

namespace PokeSharp.Core.Windowing;

public interface IWindowManager
{
    string Title { get; set; }
    Vector2 Position { get; set; }
    Rectangle Bounds { get; }

    event EventHandler<FileDroppedArgs>? FileDropped;
    event EventHandler<WindowSizeChangedArgs>? SizeChanged;
}